using Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public partial class Endpoints
    {
        private const string CsvFilename = "DocStore/SeedingFiles/{0}.csv";

        [FunctionName(nameof(SeedContainerAsync))]
        public async Task<IActionResult> SeedContainerAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var container = req.Query["container"];
            if (string.IsNullOrEmpty(container))
            {
                return new BadRequestObjectResult("Missing container");
            }

            switch (container)
            {
                case "companyHouseStatuses":
                    await SeedCompanyHouseStatuses();
                    break;
                case "fcaStatuses":
                    await SeedFcaStatuses();
                    break;
                case "baseFirmPermissions":
                    await SeedBaseFirmPermissions();
                    break;
                case "fcaRoles":
                    await SeedFcaRoles();
                    break;
                case "productMappings":
                    await SeedProductMappings();
                    break;
                default:
                    return new BadRequestObjectResult("Container not found");
            }

            return new OkObjectResult($"Seeded: {container}");
        }

        private async Task SeedCompanyHouseStatuses()
        {
            var companyHouseStatuses =
                await _csvLookupService.GetCsvDataAsync<CompanyHouseStatus>(string.Format(CsvFilename,
                    "companyHouseStatuses"));

            var tasks = new List<Task>();
            foreach (var record in companyHouseStatuses.Where(record => record.Id == "GUID"))
            {
                record.Id = Guid.NewGuid().ToString();
                tasks.Add(_companyHouseRepository.AddOrUpdateStatusAsync(record));
            }

            await Task.WhenAll(tasks);
        }

        private async Task SeedFcaStatuses()
        {
            var fcaHouseStatuses =
                await _csvLookupService.GetCsvDataAsync<FcaStatus>(string.Format(CsvFilename,
                    "fcaStatuses"));
            var tasks = new List<Task>();

            foreach (var record in fcaHouseStatuses.Where(record => record.Id == "GUID"))
            {
                record.Id = Guid.NewGuid().ToString();
                tasks.Add(_fcaStatusRepository.AddOrUpdateStatusAsync(record));
            }

            await Task.WhenAll(tasks);
        }

        private async Task SeedBaseFirmPermissions()
        {
            var baseFirmPermissions =
                await _csvLookupService.GetCsvDataAsync<BaseFirmPermission>(string.Format(CsvFilename,
                    "baseFirmPermissions"));
            var tasks = new List<Task>();
            
            foreach (var record in baseFirmPermissions.Where(record => record.Id == "GUID"))
            {
                record.Id = Guid.NewGuid().ToString();
                tasks.Add(_baseFirmPermissionRepository.SaveOrUpdateBaseFirmPermissionAsync(record));
            }

            await Task.WhenAll(tasks);
        }

        private async Task SeedFcaRoles()
        {
            var fcaRoles = await _csvLookupService.GetCsvDataAsync<FcaRole>(string.Format(CsvFilename, "fcaRoles"));
            var tasks = new List<Task>();
            
            foreach (var record in fcaRoles.Where(record => record.Id == "GUID"))
            {
                record.Id = Guid.NewGuid().ToString();
                tasks.Add(_fcaRoleRepository.SaveOrUpdateFcaRoleAsync(record));
            }

            await Task.WhenAll(tasks);
        }

        private async Task SeedProductMappings()
        {
            var productMappings =
                await _csvLookupService.GetCsvDataAsync<ProductMapping>(string.Format(CsvFilename, "productMappings"));
            var tasks = new List<Task>();
            
            foreach (var record in productMappings.Where(record => record.Id == "GUID"))
            {
                record.Id = Guid.NewGuid().ToString();
                tasks.Add(_productMappingRepository.SaveOrUpdateProductMappingAsync(record));
            }

            await Task.WhenAll(tasks);
        }
    }
}