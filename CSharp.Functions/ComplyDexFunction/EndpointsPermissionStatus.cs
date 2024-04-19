using Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    //This is all about Firm's FCA permission and statuses
    public partial class Endpoints
    {
        #region FCA Permissions

        [FunctionName(nameof(InitializeFcaPermissionsAsync))]
        public async Task<IActionResult> InitializeFcaPermissionsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var permissions = await _fcaPermissionsRepository.InitializePermissionsAsync();
            return new OkObjectResult(permissions);
        }

        [FunctionName(nameof(GetAllPermissionsAsync))]
        public async Task<IActionResult> GetAllPermissionsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var permissionsResult = await _fcaPermissionsRepository.GetAllPermissionsAsync();
            return new OkObjectResult(permissionsResult);
        }

        [FunctionName(nameof(GetPermissionsByGroupNameAsync))]
        public async Task<IActionResult> GetPermissionsByGroupNameAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetPermissionsByGroupNameAsync)}/{{groupName}}")]
            HttpRequest req,
            ILogger log,
            string groupName)
        {
            var permissionsResult = await _fcaPermissionsRepository.GetPermissionsGroupNameAsync(groupName);
            return new OkObjectResult(permissionsResult);
        }

        [FunctionName(nameof(GetPermissionsByCategoryAsync))]
        public async Task<IActionResult> GetPermissionsByCategoryAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetPermissionsByCategoryAsync)}/{{categoryName}}")]
            HttpRequest req,
            ILogger log,
            string categoryName)
        {
            var permissionsResult =
                await _fcaPermissionsRepository.GetPermissionsByCategoryNameAsync(categoryName);
            return new OkObjectResult(permissionsResult);
        }

        [FunctionName(nameof(SavePermissionAsync))]
        public async Task<IActionResult> SavePermissionAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<FcaPermission>(requestBody);
            var permissionsResult = await _fcaPermissionsRepository.AddOrUpdatePermissionAsync(data);
            return new OkObjectResult(permissionsResult);
        }

        [FunctionName(nameof(SavePermissionsMultipleAsync))]
        public async Task<IActionResult> SavePermissionsMultipleAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<List<FcaPermission>>(requestBody);
            var postPermissions = new List<FcaPermission>();

            if (data.Any())
            {
                await _fcaPermissionsRepository.DeleteAllPermissionsAsync();
            }

            foreach (var permission in data)
            {
                var existingPermission = await _fcaPermissionsRepository.GetPermissionById(permission.Id);

                if (existingPermission != null)
                {
                    permission.Id = existingPermission.Id;
                }

                var savedPermission =
                    await _fcaPermissionsRepository.AddOrUpdatePermissionAsync(permission);
                postPermissions.Add(savedPermission);
            }

            return new OkObjectResult(postPermissions);
        }

        [FunctionName(nameof(DeleteAllPermissionsAsync))]
        public async Task<IActionResult> DeleteAllPermissionsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var result = await _fcaPermissionsRepository.DeleteAllPermissionsAsync();
            return new OkObjectResult(result);
        }

        #endregion

        #region FCA Statuses

        [FunctionName(nameof(InitializeFcaStatusesAsync))]
        public async Task<IActionResult> InitializeFcaStatusesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var statusesResult = await _fcaStatusRepository.InitializeFcaStatusesAsync();
            return new OkObjectResult(statusesResult);
        }

        [FunctionName(nameof(GetAllFcaStatusesAsync))]
        public async Task<IActionResult> GetAllFcaStatusesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var permissionResult = await _fcaStatusRepository.GetAllFcaStatusesAsync();
            return new OkObjectResult(permissionResult);
        }

        [FunctionName(nameof(GetFcaStatusByActualStatusAsync))]
        public async Task<IActionResult> GetFcaStatusByActualStatusAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetFcaStatusByActualStatusAsync)}/{{actualStatus}}")]
            HttpRequest req,
            ILogger log,
            string actualStatus)
        {
            var fcaStatusResult = await _fcaStatusRepository.GetStatusByActualStatusAsync(actualStatus);
            return new OkObjectResult(fcaStatusResult);
        }

        [FunctionName(nameof(GetFcaStatusesByGeneralStatusAsync))]
        public async Task<IActionResult> GetFcaStatusesByGeneralStatusAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetFcaStatusesByGeneralStatusAsync)}/{{generalStatus}}")]
            HttpRequest req,
            ILogger log,
            string generalStatus)
        {
            var fcaStatusResult = await _fcaStatusRepository.GetFcaStatusesByGeneralStatusAsync(generalStatus);
            return new OkObjectResult(fcaStatusResult);
        }

        [FunctionName(nameof(SaveFcaStatusAsync))]
        public async Task<IActionResult> SaveFcaStatusAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<FcaStatus>(requestBody);
            var fcaStatusResult = await _fcaStatusRepository.AddOrUpdateStatusAsync(data);
            return new OkObjectResult(fcaStatusResult);
        }

        [FunctionName(nameof(SaveFcaStatusMultipleAsync))]
        public async Task<IActionResult> SaveFcaStatusMultipleAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<List<FcaStatus>>(requestBody);
            var postStatuses = new List<FcaStatus>();

            if (data.Any())
            {
                await _fcaStatusRepository.DeleteAllStatusesAsync();
            }

            foreach (var status in data)
            {
                var existingPermission = await _fcaStatusRepository.GetStatusByIdAsync(status.Id);

                if (existingPermission != null)
                {
                    status.Id = existingPermission.Id;
                }

                var savedStatus =
                    await _fcaStatusRepository.AddOrUpdateStatusAsync(status);
                postStatuses.Add(savedStatus);
            }

            return new OkObjectResult(postStatuses);
        }

        [FunctionName(nameof(DeleteFcaStatusAsync))]
        public async Task<IActionResult> DeleteFcaStatusAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete",
                Route = $"{nameof(DeleteFcaStatusAsync)}/{{actualStatus}}")]
            HttpRequest req,
            ILogger log,
            string actualStatus)
        {
            var result = await _fcaStatusRepository.DeleteStatusAsync(actualStatus);
            return new OkObjectResult(result);
        }

        #endregion
    }
}