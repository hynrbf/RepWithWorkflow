using Common.Entities;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Common.Infra
{
    public class PdfCoPdfService : IThirdPartyPdfService
    {
        private const string Endpoint = "https://api.pdf.co/v1/pdf/convert/from/html";

        private readonly string _testPdfFileFromBlob;
        private readonly string _pdfCoApiKey;
        private readonly string _blobStorageContainerName;
        private readonly bool _isEnabled;

        // TODO. To finalize if this is still needed
        public PdfCoPdfService(bool isEnabled)
        {
            _isEnabled = isEnabled;
            _pdfCoApiKey = GetEnvironmentVariable("PdfCoApiKey");
            _blobStorageContainerName = GetEnvironmentVariable("BlobStorageContainerName");

            var azureStorageBaseUrl = GetEnvironmentVariable("AzureStorageBaseUrl");
            var blobContainerUrl = $"{azureStorageBaseUrl}{_blobStorageContainerName}";

            _testPdfFileFromBlob = $"{blobContainerUrl}/WordToHtml.pdf";
        }

        public async Task<string> GeneratePdfAsync(HtmlContent htmlContent)
        {
            if (!_isEnabled)
            {
                return _testPdfFileFromBlob;
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, Endpoint);
            request.Headers.Add("x-api-key", _pdfCoApiKey);

            var payload = new
            {
                html = htmlContent.Content,
                name = htmlContent.Name
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            var obj = JObject.Parse(result);
            var pdfTempUrl = obj["url"]?.ToString();

            if (string.IsNullOrEmpty(pdfTempUrl))
            {
                return string.Empty;
            }

            return await UploadFileToBlobStorageAsync(pdfTempUrl, _blobStorageContainerName, htmlContent.Name);
        }

        private static async Task<string> UploadFileToBlobStorageAsync(string pdfTempUrl, string containerName,
            string fileName)
        {
            var client = new HttpClient();
            var pdfFileResponse = await client.GetAsync(new Uri(pdfTempUrl));
            var file = await pdfFileResponse.Content.ReadAsStreamAsync();

            await using (file)
            {
                var azureConnectionString = GetEnvironmentVariable("AzureStorageConnectionString");

                if (file.Length <= 0)
                {
                    return string.Empty;
                }

                var container = new BlobContainerClient(azureConnectionString, containerName);
                var createResponse = await container.CreateIfNotExistsAsync();

                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                {
                    await container.SetAccessPolicyAsync(PublicAccessType.Blob);
                }

                var blob = container.GetBlobClient($"{fileName}.pdf");
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await blob.UploadAsync(file, new BlobHttpHeaders { ContentType = "application/pdf" });
                return blob.Uri.ToString();
            }
        }

        private static string GetEnvironmentVariable(string key)
            => Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
    }
}