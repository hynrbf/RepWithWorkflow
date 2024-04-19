using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public partial class Endpoints
    {
        [FunctionName(nameof(UploadImageFileAsync))]
        public async Task<IActionResult> UploadImageFileAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var file = req.Form.Files["file"];

            if (!req.Form.TryGetValue("name", out var fileName))
            {
                throw new ArgumentException("Converting unnamed file.");
            }

            if (!req.Form.TryGetValue("fileExtension", out var fileExtension))
            {
                throw new ArgumentException("Converting document with no file extension.");
            }

            if (file is not { Length: > 0 })
            {
                return new OkObjectResult(false);
            }

            //allow only valid images
            string[] imageExtensions =
            {
                ".jpg", ".jpeg",
                ".png",
                ".gif",
                ".bmp",
                ".tif", ".tiff",
                ".webp",
                ".svg",
                ".ico",
                ".ai"
            };

            var fileExtensionString = fileExtension.ToString();

            if (string.IsNullOrEmpty(fileExtensionString))
            {
                throw new NullReferenceException("Type of image is null");
            }

            if (!imageExtensions.Contains($".{fileExtensionString.ToLower()}"))
            {
                throw new ArgumentException("Type of image is not allowed");
            }

            var consultantName = (await _settingRepository.GetSettingByKeyAsync("$(CONSULTANCY_NAME)")).Value;

            if (string.IsNullOrEmpty(consultantName))
            {
                throw new ArgumentException("Consultant name should be set in the Settings");
            }

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var urlPath =
                await _fileUploaderService.UploadFilesToAzureBlobStorageAsync(ms, fileName, fileExtensionString,
                    consultantName);
            ms.Close();
            return new OkObjectResult(urlPath);
        }

        [FunctionName(nameof(UploadStationaryFileAsync))]
        public async Task<IActionResult> UploadStationaryFileAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var file = req.Form.Files["file"];

            if (!req.Form.TryGetValue("name", out var fileName))
            {
                throw new ArgumentException("Converting unnamed file.");
            }

            if (!req.Form.TryGetValue("fileExtension", out var fileExtension))
            {
                throw new ArgumentException("Converting document with no file extension.");
            }

            if (file is not { Length: > 0 })
            {
                return new OkObjectResult(false);
            }

            //allow only valid stationary files
            string[] imageExtensions =
            {
                ".jpg", ".jpeg",
                ".png",
                ".gif",
                ".bmp",
                ".tif", ".tiff",
                ".webp",
                ".svg",
                ".ico",
                ".ai",
                ".doc",
                ".docx"
            };

            var fileExtensionString = fileExtension.ToString();

            if (string.IsNullOrEmpty(fileExtensionString))
            {
                throw new NullReferenceException("Type of image is null");
            }

            if (!imageExtensions.Contains($".{fileExtensionString.ToLower()}"))
            {
                throw new ArgumentException("Type of image is not allowed");
            }

            var consultantName = (await _settingRepository.GetSettingByKeyAsync("$(CONSULTANCY_NAME)")).Value;

            if (string.IsNullOrEmpty(consultantName))
            {
                throw new ArgumentException("Consultant name should be set in the Settings");
            }

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var urlPath =
                await _fileUploaderService.UploadFilesToAzureBlobStorageAsync(ms, fileName, fileExtensionString,
                    consultantName);
            ms.Close();
            return new OkObjectResult(urlPath);
        }

        [FunctionName(nameof(UploadCustomerFileAsync))]
        public async Task<IActionResult> UploadCustomerFileAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var file = req.Form.Files["file"];

            if (!req.Form.TryGetValue("name", out var fileName))
            {
                throw new ArgumentException("Uploading unnamed file.");
            }

            if (!req.Form.TryGetValue("companyNo", out var companyNumber))
            {
                throw new ArgumentException("Company number is required.");
            }

            if (!req.Form.TryGetValue("purpose", out var purpose))
            {
                throw new ArgumentException("Purpose is required.");
            }

            if (!req.Form.TryGetValue("fileExtension", out var fileExtension))
            {
                throw new ArgumentException("Uploading file with no file extension.");
            }

            if (file is not { Length: > 0 })
            {
                return new OkObjectResult(false);
            }

            string[] allowedFileExtensions =
            {
                ".jpg", ".jpeg",
                ".pdf",
                ".doc",
                ".docx"
            };

            var fileExtensionString = fileExtension.ToString();

            if (string.IsNullOrEmpty(fileExtensionString))
            {
                throw new NullReferenceException("File extension is null");
            }

            if (!allowedFileExtensions.Contains($".{fileExtensionString.ToLower()}"))
            {
                throw new ArgumentException("File type is not allowed");
            }

            var directory = "";

            // PII
            if (purpose == "PII")
            {
                directory = $"CustomerDocs/{companyNumber}/PII";
            }

            // ELI
            if (purpose == "ELI")
            {
                directory = $"CustomerDocs/{companyNumber}/ELI";
            }

            // Owners and Controllers - Individual Controllers
            if (purpose == "IndividualControllersCV")
            {
                directory = $"CustomerDocs/{companyNumber}/IndividualControllers/CV";
            }

            if (purpose == "IndividualControllersSupportingDocuments")
            {
                directory = $"CustomerDocs/{companyNumber}/IndividualControllers/SupportingDocuments";
            }

            // Owners and Controllers - Corporate Controllers
            var corporateCompanyNo = "";

            if (req.Form.TryGetValue("corporateCompanyNo", out var value))
            {
                corporateCompanyNo = value.ToString();
            }

            if (purpose == "CorporateControllersThirdCountryFirm")
            {
                directory =
                    $"CustomerDocs/{companyNumber}/CorporateControllers/{corporateCompanyNo}/SupportingDocuments/ThirdCountryFirm";
            }

            if (purpose == "CorporateControllersConglomerate")
            {
                directory =
                    $"CustomerDocs/{companyNumber}/CorporateControllers/{corporateCompanyNo}/SupportingDocuments/Conglomerate";
            }

            if (purpose == "CorporateControllersThirdConglomerate")
            {
                directory =
                    $"CustomerDocs/{companyNumber}/CorporateControllers/{corporateCompanyNo}/SupportingDocuments/ThirdConglomerate";
            }

            if (purpose == "CorporateControllersThirdCountryBanking")
            {
                directory =
                    $"CustomerDocs/{companyNumber}/CorporateControllers/{corporateCompanyNo}/SupportingDocuments/ThirdCountryBanking";
            }

            if (purpose == "CorporateControllersMaterialComplaints")
            {
                directory =
                    $"CustomerDocs/{companyNumber}/CorporateControllers/{corporateCompanyNo}/SupportingDocuments/MaterialComplaints";
            }

            if (purpose == "CorporateControllersCV" &&
                req.Form.TryGetValue("foreNameAndSurname", out var foreNameAndSurname))
            {
                directory =
                    $"CustomerDocs/{companyNumber}/CorporateControllers/{corporateCompanyNo}/CV/{foreNameAndSurname}";
            }

            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentNullException(
                    "directory is null, so please define the purpose correctly in the uploading file");
            }

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var urlPath =
                await _fileUploaderService.UploadCustomerFilesToAzureBlobStorageAsync(ms, directory, fileName,
                    fileExtensionString);
            ms.Close();
            return new OkObjectResult(urlPath);
        }
    }
}