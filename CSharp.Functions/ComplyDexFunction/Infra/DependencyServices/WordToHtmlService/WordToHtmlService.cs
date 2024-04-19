using System;
using System.IO;
using System.Linq;
using Aspose.Words;
using Aspose.Words.Saving;
using Microsoft.AspNetCore.Http;
using Common.Entities;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Common;
using Common.Infra;

namespace Api.Infra
{
    //Note. We wont use 3rd Pary service here because this doesnt call outside
    //api rest service. instead we inject the code and make it our own
    public class WordToHtmlService : AsposeLicensedServiceBase, IWordToHtmlService
    {
        private readonly IHtmlRepository _htmlRepository;
        private readonly string _containerName;
        private readonly string _containerConnectionString;
        private readonly string _azureStorageBaseUrl;
        private readonly string _blobStorageContainerName;

        public WordToHtmlService(IHtmlRepository htmlRepository) : base()
        {
            _htmlRepository = htmlRepository;
            _containerName =
                Environment.GetEnvironmentVariable("BlobStorageContainerName", EnvironmentVariableTarget.Process);
            _containerConnectionString =
                Environment.GetEnvironmentVariable("AzureStorageConnectionString", EnvironmentVariableTarget.Process);
            _azureStorageBaseUrl = 
                Environment.GetEnvironmentVariable("AzureStorageBaseUrl", EnvironmentVariableTarget.Process);
            _blobStorageContainerName = Environment.GetEnvironmentVariable("BlobStorageContainerName", EnvironmentVariableTarget.Process);
        }

        public async Task<string> ConvertSavedHtmlToWordAsync()
        {
            if (_htmlRepository == null)
            {
                throw new NullReferenceException(
                    $"The {nameof(IHtmlRepository)} should not be null from {nameof(WordToHtmlService)}.{nameof(ConvertSavedHtmlToWordAsync)}");
            }

            if (string.IsNullOrEmpty(_containerName) || string.IsNullOrEmpty(_containerConnectionString))
            {
                throw new NullReferenceException(
                    $"The container name and connection should not be null from {nameof(WordToHtmlService)}.{nameof(ConvertSavedHtmlToWordAsync)}");
            }

            //get the first saved html from db
            var listOfHtmlContents = await _htmlRepository.GetHtmlSourcesAsync();
            var firstContent = listOfHtmlContents.FirstOrDefault();

            if (firstContent == null)
            {
                return String.Empty;
            }

            //write the contents of html into memory
            using var memStream = new MemoryStream();
            await using var writer = new StreamWriter(memStream, Encoding.UTF8, 1024, true);
            await writer.WriteAsync(firstContent.Content);

            //load the memory stream as word doc, then write it as doc in another memory stream

            var doc = new Document(memStream);
            var fileType = firstContent.ContentType;
            var saveFormat = fileType switch
            {
                "docx" => SaveFormat.Docx,
                "doc" => SaveFormat.Doc,
                _ => throw new ArgumentException("Content type is not supported")
            };
            using var memStreamConverted = new MemoryStream();
            doc.Save(memStreamConverted, saveFormat);

            //save the actually doc in blob container
            var containerClient = new BlobContainerClient(_containerConnectionString, _containerName);
            var extension = fileType == "docx" ? ".docx" : ".doc";
            var blobName = $"ComplyDexToDel/convertedBack{extension}";
            var blob = containerClient.GetBlobClient(blobName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            memStreamConverted.Position = 0;
            await containerClient.UploadBlobAsync(blobName, memStreamConverted);
            return $"{_azureStorageBaseUrl}{_blobStorageContainerName}/{blobName}";
        }

        public async Task<HtmlContent> ConvertWordToHtmlThenSaveToDbAsync(IFormFile file, string fileExtension,
            string documentName)
        {
            if (_htmlRepository == null)
            {
                throw new NullReferenceException(
                    $"The {nameof(IHtmlRepository)} should not be null from {nameof(WordToHtmlService)}.{nameof(ConvertWordToHtmlThenSaveToDbAsync)}");
            }

            using var ms = new MemoryStream();
            //Docs are here https://products.aspose.com/words/net/
            await file.CopyToAsync(ms);
            var doc = new Document(ms);
            var saveOptions = new HtmlSaveOptions
            {
                ExportImagesAsBase64 = true
            };

            using var streamHtml = new MemoryStream();
            doc.Save(streamHtml, saveOptions);
            var html = Encoding.UTF8.GetString(streamHtml.ToArray());
            var htmlContentModel = await _htmlRepository.UpdateHtmlSourceAsync(new HtmlContent
            {
                Id = Guid.NewGuid().ToString(),
                Name = documentName,
                Content = html,
                ContentType = fileExtension
            });
            return htmlContentModel;
        }
    }
}