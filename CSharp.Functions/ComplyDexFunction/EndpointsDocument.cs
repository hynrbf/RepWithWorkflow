using Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace Api
{
    //This is all about document word manipulation
    //converting word to html, html to pdf, replacing values in word etc..
    public partial class Endpoints
    {
        #region Document's Html Content CRUD

        [FunctionName(nameof(GetAllHtmlSourceDocumentsAsync))]
        public async Task<IActionResult> GetAllHtmlSourceDocumentsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var htmlSources = await _htmlRepository.GetHtmlSourcesAsync();
            return new OkObjectResult(htmlSources);
        }

        [FunctionName(nameof(SaveHtmlSourceDocumentAsync))]
        public async Task<IActionResult> SaveHtmlSourceDocumentAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<HtmlContent>(requestBody);
            var postHtmlSourceDocument = await _htmlRepository.UpdateHtmlSourceAsync(data);
            return new OkObjectResult(postHtmlSourceDocument);
        }

        #endregion

        #region Document's Templating Service

        [FunctionName(nameof(GetDocumentHtmlContentAsync))]
        public async Task<IActionResult> GetDocumentHtmlContentAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetDocumentHtmlContentAsync)}/{{name}}")]
            HttpRequest req,
            ILogger log,
            string name)
        {
            var htmlSource = await _htmlRepository.GetHtmlSourcesAsync(name);
            return new OkObjectResult(htmlSource);
        }

        #endregion

        #region WordToHtml Service

        [FunctionName(nameof(UploadDocumentAsync))]
        public async Task<IActionResult> UploadDocumentAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            //https://soltisweb.com/blog/detail/2020-11-10-howtopostafiletoazurefunctionin3minutes
            var file = req.Form.Files["file"];

            if (!req.Form.TryGetValue("name", out var docName))
            {
                throw new ArgumentException("Converting unnamed document.");
            }

            if (!req.Form.TryGetValue("fileExtension", out var fileExtension))
            {
                throw new ArgumentException("Converting document with no file extension.");
            }

            if (file is not { Length: > 0 })
            {
                return new OkObjectResult(false);
            }

            var consultantName = (await _settingRepository.GetSettingByKeyAsync("$(CONSULTANCY_NAME)")).Value;

            if (string.IsNullOrEmpty(consultantName))
            {
                throw new ArgumentException("Consultant name should be set in the Settings");
            }

            if (file is not { Length: > 0 })
            {
                return new OkObjectResult(false);
            }

            await using var ms = new MemoryStream();
            //Docs are here https://products.aspose.com/words/net/
            await file.CopyToAsync(ms);

            // upload here
            await using (ms)
            {
                var containerName =
                    Environment.GetEnvironmentVariable("BlobStorageContainerName", EnvironmentVariableTarget.Process);
                var azureConnectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString",
                    EnvironmentVariableTarget.Process);

                if (ms.Length <= 0)
                {
                    return new OkObjectResult(string.Empty);
                }

                var container = new BlobContainerClient(azureConnectionString, containerName);
                var createResponse = await container.CreateIfNotExistsAsync();

                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                {
                    await container.SetAccessPolicyAsync(PublicAccessType.Blob);
                }

                var blob = container.GetBlobClient(
                    $"ConversionProcess/raw.word.docs/{consultantName}/{docName}.{fileExtension}");
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                ms.Position = 0;
                await blob.UploadAsync(ms,
                    new BlobHttpHeaders
                        { ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" });

                var htmlContentModel = await _htmlRepository.UpdateHtmlSourceAsync(new HtmlContent
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = docName,
                    ContentType = fileExtension,
                    FileUrl = blob.Uri.ToString(),
                    Version = "1.0", // version 1 for now
                    Consultant = consultantName // for now
                });

                return new OkObjectResult(htmlContentModel);
            }
        }

        [FunctionName(nameof(ConvertHtmlToDocAsync))]
        public async Task<IActionResult> ConvertHtmlToDocAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var result = await _wordToHtmlService.ConvertSavedHtmlToWordAsync();
            await Task.Delay(500); //just a pause
            return new OkObjectResult(result);
        }

        #endregion
    }
}