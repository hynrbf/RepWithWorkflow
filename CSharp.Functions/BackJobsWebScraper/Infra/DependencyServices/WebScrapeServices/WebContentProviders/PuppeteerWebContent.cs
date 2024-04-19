using Common.Entities;
using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Common.Infra
{
    public class PuppeteerWebContent : RestServiceBase, IWebContentProvider
    {
        private readonly string _baseUrl = AppSettingsProvider.Instance.GetValue(AppConstants.PuppeteerScraperApi);
        private readonly MemoryCache _memoryCache = MemoryCache.Default;

        public async Task<HtmlDocument> GetWebContent(string url)
        {
            var htmlDoc = new HtmlDocument();

            var puppeteerContent = await GetWebContentAsync(url);
            if (!string.IsNullOrEmpty(puppeteerContent.Error))
            {
                throw new Exception($"Puppeteer error in URL: {url}. ERROR: {puppeteerContent.Error}");
            }

            htmlDoc.LoadHtml(puppeteerContent.HtmlContent);
            return htmlDoc;
        }

        protected override HttpRequestMessage CreateRequestMessageGet(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            return request;
        }

        protected override HttpRequestMessage CreateRequestMessagePost(string endpoint, HttpContent httpContent)
        {
            throw new NotImplementedException();
        }
        
        private async Task<PuppeteerScrapeResult> GetWebContentAsync(string url)
        {
            var endpoint = $"{_baseUrl}?target={url}";

            if (_memoryCache.Get(endpoint) is PuppeteerScrapeResult cachedValue)
            {
                return cachedValue;
            }

            var output = await GetRemoteAsync(endpoint,
                async response => await HandleFailureAsync<PuppeteerScrapeResult>(endpoint, response));

            _memoryCache.AddOrGetExisting(endpoint, output,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return output;
        }
    }
}
