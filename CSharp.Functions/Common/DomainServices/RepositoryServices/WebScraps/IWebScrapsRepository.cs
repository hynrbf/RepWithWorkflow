using Common.Entities;

namespace Common
{
    public interface IWebScrapsRepository
    {
        Task<List<WebScrap>> GetWebScrapsForScrapeAsync();
        Task<List<WebScrap>> GetWebScrapsWithRootUrlAsync(string rootUrl);
        Task<List<WebScrap>> GetChildWebScrapsAsync();
        Task<WebScrap?> GetWebScrapByIdAsync(string id);
        Task<WebScrap?> GetWebScrapByUrlAsync(string url);
        Task<WebScrap> SaveWebScrapAsync(WebScrap webScrap);
        Task<bool> DeleteAllWebScrapsAsync();
    }
}