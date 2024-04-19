using Common;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Api
{
    public partial class Endpoints
    {
        private const string CryptoLookupFileName = "DocStore/LookupFiles/Crypto_Mapping.csv";

        [FunctionName(nameof(GetScrappedWebContentFromKeywordAsync))]
        public async Task<IActionResult> GetScrappedWebContentFromKeywordAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var payload = JsonConvert.DeserializeObject<ScrapContentPayload>(requestBody);
            var url = payload?.Url ?? string.Empty;
            var keyword = payload?.Keyword ?? string.Empty;
            var scrapItem = _webContentsScrapping.GetContentFromUrlByKeyword(url, keyword);
            return new OkObjectResult(scrapItem);
        }

        [FunctionName(nameof(GetScrappedWebContent))]
        public IActionResult GetScrappedWebContent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var url = req.Query["url"];
            var scrapItem = _webContentsScrapping.GetScrappedWebContent(url);
            return new OkObjectResult(scrapItem);
        }

        [FunctionName(nameof(RegisterMediaAsync))]
        public async Task<IActionResult> RegisterMediaAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            string customerId = req.Query["customerId"];
            string mediaId = req.Query["mediaId"]; // Media Marketing Id
            string url = req.Query["url"]; // Media Marketing Account URL

            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(mediaId) || string.IsNullOrEmpty(url))
            {
                log.LogError("Customer Id or media Id or url parameter is not found");
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            var webScrapEntry = await _webScrapeService.RegisterMedia(customerId, mediaId, url);
            return new OkObjectResult(webScrapEntry.Id);
        }

        [FunctionName(nameof(GetCryptoNamesAsync))]
        public async Task<IActionResult> GetCryptoNamesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var exception = req.Query["exception"];

            var cryptoNames = await _csvLookupService.GetCryptoNamesAsync(exception, CryptoLookupFileName);
            return new OkObjectResult(cryptoNames);
        }

        private static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var validatedUri) &&
                   (validatedUri.Scheme == Uri.UriSchemeHttp || validatedUri.Scheme == Uri.UriSchemeHttps);
        }

    }
}