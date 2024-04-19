using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Linq;
using Azure.Storage.Blobs;
using System.IO;

namespace Api
{
    //WARNING! Please read method 'SetUrlsForDeletion'
    public partial class Endpoints
    {
        private string _storageBaseUrlToBeDeleted;
        private string _databaseName;
        private bool _isAllowCleaning;
        private BlobContainerClient _containerClientToBeDeleted;

        [FunctionName(nameof(CleanAppAsync))]
        public async Task<bool> CleanAppAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            if (!_isAllowCleaning)
            {
                throw new Exception("You can't run this function, this is a live environment");
            }

            if (_databaseName.Contains("live"))
            {
                throw new Exception("You can't run this function, this is a live environment");
            }

            var customers = (await _customerRepository.GetCustomersAsync()).ToList();

            foreach (var customer in customers)
            {
                var customerEmail = customer.Email;
                await CleanCustomerAsync(customer.Id, customerEmail, log, customer.CompanyNumber,
                    customer.FirmReferenceNumber);
            }

            var commentsList = await _commentRepository.GetCommentsAsync();

            foreach (var comment in commentsList)
            {
                await _commentRepository.DeleteCommentByIdAsync(comment.Id);
            }

            //delete as well, appointedRepresentatives and customerAppointedRepresentatives and saveFields container?

            return true;
        }

        [FunctionName(nameof(CleanAppByIdEmailAsync))]
        public async Task<bool> CleanAppByIdEmailAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var customer = await _customerRepository.GetCustomerByEmailAsync(requestBody);

            if (string.IsNullOrEmpty(requestBody))
            {
                throw new NullReferenceException("The request body should not be null");
            }

            if (!_isAllowCleaning)
            {
                throw new Exception("You can't run this function, this is a live environment");
            }

            if (_databaseName.Contains("live"))
            {
                throw new Exception("You can't run this function, this is a live environment");
            }

            if (customer == null)
            {
                throw new NullReferenceException(
                    $"Please check your email input {requestBody}. This is not existing in db.");
            }

            await CleanCustomerAsync(customer.Id, customer.Email, log, customer.CompanyNumber,
                customer.FirmReferenceNumber);
            return true;
        }

        private async Task CleanCustomerAsync(string id, string customerEmail, ILogger log, string companyNo,
            string firmRefNo)
        {
            if (string.IsNullOrEmpty(customerEmail))
            {
                return;
            }

            // TODO. remove auth0 users except chito@suntech.gi
            //if you delete, make sure just leave chito@suntech.gi in auth0 users
            if (customerEmail.ToLower().Contains("chito@suntech.gi"))
            {
                return;
            }

            if (customerEmail.Contains(" ("))
            {
                customerEmail = customerEmail[..customerEmail.IndexOf(" (", StringComparison.Ordinal)];
            }

            var isCustomerDeleted =
                await _customerRepository.DeleteCustomerByIdEmailAsync(id, customerEmail);
            var isSchemaAnswersDeleted = false;

            if (!string.IsNullOrEmpty(id))
            {
                isSchemaAnswersDeleted =
                    await _schemaAnswerRepository.DeleteSchemaAnswerByIdAsync(id);
            }

            if (!isCustomerDeleted || !isSchemaAnswersDeleted)
            {
                log.LogInformation("either customer or schema answer is not deleted");
            }

            if (await _auth0Service.DeleteUserAsync(id))
            {
                log.LogInformation($"The user {customerEmail} was deleted in Auth0");
            }

            if (!string.IsNullOrEmpty(companyNo))
            {
                await _organizationalStructureRepository.DeleteEmployeeAsync(companyNo);
            }

            if (!string.IsNullOrEmpty(firmRefNo))
            {
                await _appointedRepresentativeRepository.DeleteAllAppointedRepresentativesAsync(firmRefNo);
            }

            var htmlFileLocation = $"ConversionProcess/html.files/{customerEmail}/";
            var pdfFileLocation = $"ConversionProcess/pdf.files/{customerEmail}/";
            var pdfScomFileLocation = $"ConversionProcess/pdf.files.scom/{customerEmail}/";

            var isHtmlFolderDeleted = await DeleteFolderFromStorageAsync(htmlFileLocation);
            var isPdfFolderDeleted = await DeleteFolderFromStorageAsync(pdfFileLocation);
            var isPdfScomFolderDeleted = await DeleteFolderFromStorageAsync(pdfScomFileLocation);

            if (!isHtmlFolderDeleted || !isPdfFolderDeleted || !isPdfScomFolderDeleted)
            {
                log.LogInformation("either html or pdf folder is not deleted");
            }
        }

        private async Task<bool> DeleteFolderFromStorageAsync(string blobLocation)
        {
            var blobsToDeleteExist = false;

            await foreach (var blobItem in _containerClientToBeDeleted.GetBlobsAsync(prefix: blobLocation))
            {
                var blobClient = _containerClientToBeDeleted.GetBlobClient(blobItem.Name);

                if (await blobClient.DeleteIfExistsAsync())
                {
                    blobsToDeleteExist = true;
                }
            }

            return blobsToDeleteExist;
        }

        [FunctionName(nameof(BackUpAppFullDataAsync))]
        public async Task<IActionResult> BackUpAppFullDataAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            //algo
            // check first the value "FunctionAppUrl"
            // read the customers info           
            // delete html files and pdf files
            // delete customer and schema answers db
            // remove auth0 users except chito@suntech.gi
            await Task.Delay(500);
            return new OkObjectResult(true);
        }

        #region Only Run If container deletion is NOT success

        //This is only run if the organization container is not deleted success
        [FunctionName(nameof(CleanOrganizationContainerAsync))]
        public async Task<bool> CleanOrganizationContainerAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            if (!_isAllowCleaning)
            {
                throw new Exception("You can't run this function, this is a live environment");
            }

            if (_databaseName.Contains("live"))
            {
                throw new Exception("You can't run this function, this is a live environment");
            }

            var organizationEmployeeList = await _organizationalStructureRepository.GetEmployeesAsync();
            var deleteTasks = (from emp in organizationEmployeeList
                where !string.IsNullOrEmpty(emp.CompanyNo)
                select _organizationalStructureRepository.DeleteEmployeeAsync(emp.CompanyNo)).ToList();

            await Task.WhenAll(deleteTasks);
            return true;
        }

        [FunctionName(nameof(CleanAppointedRepresentativesContainerAsync))]
        public async Task<bool> CleanAppointedRepresentativesContainerAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            if (!_isAllowCleaning)
            {
                throw new Exception("You can't run this function, this is a live environment");
            }

            if (_databaseName.Contains("live"))
            {
                throw new Exception("You can't run this function, this is a live environment");
            }

            var appointedRepresentativesList =
                await _appointedRepresentativeRepository.GetAllAppointedRepresentativesAsync();

            foreach (var item in appointedRepresentativesList)
            {
                await _appointedRepresentativeRepository.DeleteAppointedRepresentativeByIdAsync(item.Id);
            }

            return true;
        }

        [FunctionName(nameof(CleanCustomerReplicaAsync))]
        public async Task<bool> CleanCustomerReplicaAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            if (!_isAllowCleaning)
            {
                throw new Exception("You can't run this function, this is a live environment");
            }

            if (_databaseName.Contains("live"))
            {
                throw new Exception("You can't run this function, this is a live environment");
            }

            await _customerFcaReplicationRepository.DeleteAllCustomerFcaReplicationAsync();
            await _customerReplicationRepository.DeleteAllCustomerReplicationAsync();
            return true;
        }

        #endregion

        private void SetUrlsForDeletion()
        {
            //WARNING! Please make sure these values are correctly set into staging
            var databaseName =
                Environment.GetEnvironmentVariable("Database", EnvironmentVariableTarget.Process);
            _storageBaseUrlToBeDeleted =
                Environment.GetEnvironmentVariable("AzureStorageBaseUrl", EnvironmentVariableTarget.Process);
            var appCleanEnabled =
                Environment.GetEnvironmentVariable("DevAdminFullCleanEnabled", EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(appCleanEnabled))
            {
                throw new NullReferenceException(
                    $"{nameof(databaseName)} should not be null in {nameof(Endpoints)} dev admin ctr.");
            }

            if (!bool.TryParse(appCleanEnabled, out var isAllowCleaning))
            {
                throw new NullReferenceException(
                    $"{nameof(appCleanEnabled)} should not be null in {nameof(Endpoints)} dev admin ctr.");
            }

            _databaseName = databaseName;
            _isAllowCleaning = isAllowCleaning;

            if (string.IsNullOrEmpty(_storageBaseUrlToBeDeleted))
            {
                throw new NullReferenceException(
                    $"{nameof(_storageBaseUrlToBeDeleted)} should not be null in {nameof(Endpoints)} dev admin ctr.");
            }

            var storageConnection =
                Environment.GetEnvironmentVariable("AzureStorageConnectionString", EnvironmentVariableTarget.Process);
            var containerName =
                Environment.GetEnvironmentVariable("BlobStorageContainerName", EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(storageConnection) ||
                string.IsNullOrEmpty(containerName))
            {
                throw new NullReferenceException(
                    "Storage connection string, container name and callback url should be set in dev admin ctr.");
            }

            var blobServiceClient = new BlobServiceClient(storageConnection);
            _containerClientToBeDeleted = blobServiceClient.GetBlobContainerClient(containerName);
        }
    }
}