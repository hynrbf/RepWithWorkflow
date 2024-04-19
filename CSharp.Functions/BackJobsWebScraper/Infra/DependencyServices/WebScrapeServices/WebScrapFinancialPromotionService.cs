using BackJobsWebScraper.Utilities;
using Common;
using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackJobsWebScraper.Infra
{
    public class WebScrapFinancialPromotionService : IWebScrapFinancialPromotionService
    {
        private readonly IWebScrapsRepository _webScrapsRepository;
        private readonly IFinancialPromotionRepository _financialPromotionRepository;
        private readonly IWebContentParser _webContentParser;

        private List<string> _namesForWarning = new();

        private const int DailyFinancialPromotionTarget = 20;
        private int _webScrapToFinancialPromotionCount; // Web scraped URL only

        public WebScrapFinancialPromotionService(IWebScrapsRepository webScrapsRepository,
            IFinancialPromotionRepository financialPromotionRepository, IWebContentParser webContentParser)
        {
            _webScrapsRepository = webScrapsRepository;
            _financialPromotionRepository = financialPromotionRepository;
            _webContentParser = webContentParser;
        }

        public async Task RunWebScrapToFinancialPromotion(List<string> namesForWarning)
        {
            _namesForWarning = namesForWarning;
            _webScrapToFinancialPromotionCount = 0;

            var financialPromotions =
                (await _financialPromotionRepository.GetAllFinancialPromotionsAsync()).ToList();
            var savingTasks = new List<Task>();
            foreach (var financialPromotion in financialPromotions)
            {
                if (financialPromotion.IsRootUrl && HasNewFinancialPromotionsSlots(financialPromotions,
                        financialPromotion.CustomerId, financialPromotion.ContentUrl))
                {
                    var webScrapSubs =
                        await _webScrapsRepository.GetWebScrapsWithRootUrlAsync(financialPromotion.ContentUrl);
                    var customerFinancialPromotions =
                        financialPromotions.Where(fp => fp.CustomerId == financialPromotion.CustomerId);
                    //Exclude already added sub urls
                    webScrapSubs.RemoveAll(w => customerFinancialPromotions.Any(f =>
                        f.ContentUrl.Equals(w.Url, StringComparison.InvariantCultureIgnoreCase)));
                    await AddFinancialPromotions(financialPromotion, webScrapSubs.Take(DailyFinancialPromotionTarget));
                }

                if (!IsScheduledForScrape(financialPromotion.LastScrapeDate))
                {
                    continue;
                }

                var updatingFinancialPromotion = await _financialPromotionRepository.GetFinancialPromotionByIdAsync(financialPromotion.Id ?? "");
                if (updatingFinancialPromotion == null)
                {
                    continue;
                }

                var hasUpdatedContent = await SetContentFromScrapeRun(updatingFinancialPromotion, financialPromotion.ContentUrl);
                if (hasUpdatedContent)
                {
                    updatingFinancialPromotion.Owner = financialPromotion.CustomerId;
                    updatingFinancialPromotion.Moderator = financialPromotion.CustomerId;
                    savingTasks.Add(_financialPromotionRepository.SaveOrUpdateFinancialPromotionAsync(updatingFinancialPromotion));
                }
            }
            await Task.WhenAll(savingTasks);
            Console.WriteLine($"WebScraps to Financial Promotions: {_webScrapToFinancialPromotionCount}");
        }

        private static bool IsScheduledForScrape(DateTime? lastScrapeDate)
        {
            const int intervalInHours = 7 * 24; // 7 days, should be aligned with WebScraps
            if (lastScrapeDate == null)
            {
                return true;
            }

            var timeDifference = DateTime.UtcNow - lastScrapeDate.Value;
            return timeDifference.Hours >= intervalInHours;
        }

        private async Task<List<WebScrap>> GetWebScrapSubs(WebScrap webScrap)
        {
            var allWebScraps = await _webScrapsRepository.GetChildWebScrapsAsync();

            var helper = new HierarchyHelper(allWebScraps);
            return helper.GetAllChildren(webScrap.Id);
        }

        private async Task AddFinancialPromotions(FinancialPromotion financialPromotion,
            IEnumerable<WebScrap> webScraps)
        {
            var savingTasks = new List<Task>();
            foreach (var webScrap in webScraps)
            {
                var newFinancialPromotion = new FinancialPromotion
                {
                    MediaOutlet = financialPromotion.MediaOutlet,
                    Owner = financialPromotion.CustomerId, //TEMP so not Undefined User
                    Moderator = financialPromotion.CustomerId, //TEMP so not Undefined User
                    IsLive = true,
                    Type = financialPromotion.Type,
                    CustomerId = financialPromotion.CustomerId,

                    Id = Guid.NewGuid().ToString(),
                    ContentUrl = webScrap.Url,
                    IsRootUrl = false,
                };

                await SetContentFromScrapeRun(newFinancialPromotion, webScrap.Url, webScrap.Id);
                savingTasks.Add(
                    _financialPromotionRepository.SaveOrUpdateFinancialPromotionAsync(newFinancialPromotion));
            }

            await Task.WhenAll(savingTasks);
        }

        private static bool HasNewFinancialPromotionsSlots(IEnumerable<FinancialPromotion> financialPromotions,
            string customerId, string rootUrl)
        {
            var rootUrlNonWww = rootUrl.Replace("://www.", "://");
            var customerFinancialPromotionsWithRootUrl = financialPromotions
                .Where(fp =>
                    fp.CustomerId == customerId &&
                    (fp.ContentUrl.StartsWith(rootUrl) || fp.ContentUrl.StartsWith(rootUrlNonWww))).ToList();
            if (customerFinancialPromotionsWithRootUrl.Count < DailyFinancialPromotionTarget)
            {
                return true;
            }

            var lastFinancialPromotion = customerFinancialPromotionsWithRootUrl.MaxBy(cfp => cfp.CreatedAt);
            var lastFinancialPromotionCreatedDate = DateHelper.ConvertEpochToDateTime(lastFinancialPromotion.CreatedAt);
            return lastFinancialPromotionCreatedDate.AddHours(24) < DateTime.UtcNow;
        }

        private async Task<bool> SetContentFromScrapeRun(FinancialPromotion financialPromotion, string url,
            string webScrapId = "")
        {
            WebScrap webScrap;
            if (!string.IsNullOrEmpty(webScrapId))
            {
                // Faster, if WebScrapId is known
                webScrap = await _webScrapsRepository.GetWebScrapByIdAsync(webScrapId);
            }
            else
            {
                webScrap = await _webScrapsRepository.GetWebScrapByUrlAsync(url);
            }

            // Get latest web scrap
            var webScrapeRun = webScrap?.WebScrapeRuns.MaxBy(wr => wr.ScrapeDate);
            if (webScrapeRun == null)
            {
                return false;
            }

            // Check if FP has already the latest web scrap
            if (financialPromotion.Content?.ScrapeRunId != null &&
                webScrapeRun.Id == financialPromotion.Content?.ScrapeRunId)
            {
                return false;
            }

            var content = _webContentParser.GetStructuredWebContent(webScrapeRun, _namesForWarning);
            financialPromotion.Content = content;
            financialPromotion.EditorContent = new EditorContent(); // Important or won't be displayed

            financialPromotion.LastScrapeDate = DateTime.Now;

            _webScrapToFinancialPromotionCount++;

            return true;
        }
    }
}
