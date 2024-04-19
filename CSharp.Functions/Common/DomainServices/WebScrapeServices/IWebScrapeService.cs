using Common.Entities;

namespace Common
{
    public interface IWebScrapeService
    {
        Task<WebScrap> RegisterMedia(string customerId, string mediaId, string url);

        Task<bool> RunWebScrape(int scrapeInterval, int subUrlsLimit, int subUrlsPerRunLimit,
            Func<MediaOutletType, IWebContentProvider> webContentProviderDelegate);
    }
}