using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Common;

namespace Api.Infra
{
    public class FileUploaderService : IFileUploaderService
    {
        private readonly IBlobContainerService _blobService;

        public FileUploaderService(IBlobContainerService blobService)
        {
            _blobService = blobService;
        }

        public async Task<string> UploadFilesToAzureBlobStorageAsync(MemoryStream fileInMemory, string fileName,
            string fileExtension, string consultantName)
        {
            var urlPathSaved = "";

            await using (fileInMemory)
            {
                if (fileInMemory.Length <= 0)
                {
                    return "";
                }

                var container = _blobService.BlobContainerClient ??
                                throw new NullReferenceException("Blob client is null");
                var createResponse = await container.CreateIfNotExistsAsync();

                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                {
                    await container.SetAccessPolicyAsync(PublicAccessType.Blob);
                }

                var blob = container.GetBlobClient(
                    $"FileImages/{consultantName}/{fileName}.{fileExtension}");
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                fileInMemory.Position = 0;
                var mimeType = $"image/{fileExtension}"; //e.g. image/jpg
                await blob.UploadAsync(fileInMemory,
                    new BlobHttpHeaders
                        { ContentType = mimeType });
                urlPathSaved = blob.Uri.ToString();
            }

            return urlPathSaved;
        }

        public async Task<string> UploadCustomerFilesToAzureBlobStorageAsync(MemoryStream fileInMemory, string savePath, string fileName,
            string fileExtension)
        {
            var urlPathSaved = "";

            await using (fileInMemory)
            {
                if (fileInMemory.Length <= 0)
                {
                    return "";
                }

                var container = _blobService.BlobContainerClient ??
                                throw new NullReferenceException("Blob client is null");
                var createResponse = await container.CreateIfNotExistsAsync();

                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                {
                    await container.SetAccessPolicyAsync(PublicAccessType.Blob);
                }

                var blob = container.GetBlobClient(
                    $"{savePath}/{fileName}.{fileExtension}");
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                fileInMemory.Position = 0;

                var mimeType = "";

                if (fileExtension == "doc" || fileExtension == "docx")
                {
                    mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                }
                else if (fileExtension == "jpg" || fileExtension == "jpeg")
                {
                    mimeType = $"image/{fileExtension}"; //e.g. image/jpg
                }
                
                await blob.UploadAsync(fileInMemory,
                    new BlobHttpHeaders
                    { ContentType = mimeType });
                urlPathSaved = blob.Uri.ToString();
            }

            return urlPathSaved;
        }
    }
}