using Common.Entities;
using HtmlAgilityPack;
using System.Collections.Concurrent;

namespace Common.Infra
{
    public class WebScrapeService : IWebScrapeService
    {
        private readonly IWebScrapsRepository _webScrapsRepository;
        private readonly IFinancialPromotionRepository _financialPromotionRepository;

        private Func<MediaOutletType, IWebContentProvider> _webContentProviderDelegate;
        private List<WebScrap> _dbWebScraps;
        private ConcurrentBag<WebScrap> _processedWebScraps; // Thread-safe

        private int _subUrlsLimit = 20;
        private int _subUrlsPerRunLimit = 20;
        private int _scrapeInterval = 7;
        private int _scrapeCount; // No. of actual request to the URL

        private DateTime? _rootWebScrapCompletedDate;

        public WebScrapeService(IWebScrapsRepository webScrapsRepository,
            IFinancialPromotionRepository financialPromotionRepository)
        {
            _webScrapsRepository = webScrapsRepository;
            _financialPromotionRepository = financialPromotionRepository;
        }

        public async Task<WebScrap> RegisterMedia(string customerId, string mediaId, string url)
        {
            var webScrapEntry = await _webScrapsRepository.GetWebScrapByUrlAsync(url) ??
                                await RegisterWebScrapUrl(url);

            var financialPromotion = new FinancialPromotion
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = customerId,
                MediaOutlet = mediaId,
                ContentUrl = url,
                IsRootUrl = true,
                IsLive = true
            };

            await _financialPromotionRepository.SaveOrUpdateFinancialPromotionAsync(financialPromotion);

            return webScrapEntry;
        }

        public async Task<bool> RunWebScrape(int scrapeInterval, int subUrlsLimit, int subUrlsPerRunLimit,
            Func<MediaOutletType, IWebContentProvider> webContentProviderDelegate)
        {
            _subUrlsLimit = subUrlsLimit;
            _subUrlsPerRunLimit = subUrlsPerRunLimit;
            _scrapeInterval = scrapeInterval;
            _webContentProviderDelegate = webContentProviderDelegate;

            _processedWebScraps = new ConcurrentBag<WebScrap>();

            _scrapeCount = 0;

            var allWebScraps = await _webScrapsRepository.GetWebScrapsForScrapeAsync();

            var rootWebScrap = allWebScraps.FirstOrDefault(w => string.IsNullOrEmpty(w.ParentId));
            if (rootWebScrap == null)
            {
                Console.WriteLine("No URL to process.");
                return false;
            }

            _rootWebScrapCompletedDate = rootWebScrap.UpdatedDate ?? DateTime.UtcNow.AddDays(-_scrapeInterval);

            _dbWebScraps = allWebScraps.ToList();
            Console.WriteLine($"Web Scraps for {rootWebScrap.Url}: {_dbWebScraps.Count}");

            foreach (var webScrap in _dbWebScraps)
            {
                var currentUrl = webScrap.Url;
                if (!IsValidUrl(currentUrl))
                {
                    continue;
                }

                var rootUrl = string.IsNullOrEmpty(webScrap.ParentId) ? currentUrl : webScrap.RootUrl;
                if (IsSubUrlsPerRunLimitReached() || !IsValidUrl(rootUrl))
                {
                    continue;
                }

                var subUrlsResult = await AddWebScrapContent(webScrap, rootUrl);

                var subUrls = subUrlsResult.Select(su => su).ToList();
                await ScrapeSubUrls(webScrap, subUrls, rootUrl);
            }

            /*// This is fast but results to request being blocked; TODO: revisit
            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = 10
            };
            await Parallel.ForEachAsync(_dbWebScraps, parallelOptions, async (webScrap, token) =>
            {
                var currentUrl = webScrap.Url;
                var rootUrl = string.IsNullOrEmpty(webScrap.ParentId) ? currentUrl : webScrap.RootUrl;
                if (IsValidUrl(rootUrl) && IsValidUrl(currentUrl) && !IsScrapeLimitReached(rootUrl))
                {
                    var subUrlsResult = await AddWebScrapContent(webScrap, rootUrl);
                    var subUrls = subUrlsResult.Select(su => su).ToList();
                    await ScrapeSubUrls(webScrap, subUrls, rootUrl);
                }
            });*/

            Console.WriteLine($"Scrape Count: {_scrapeCount}");
            if (_scrapeCount == 0 || IsSubUrlsLimitReached())
            {
                Console.WriteLine($"Scrape complete for {rootWebScrap.Url}.");

                // Get the full WebScrap
                rootWebScrap = await _webScrapsRepository.GetWebScrapByIdAsync(rootWebScrap.Id) ?? rootWebScrap;
                rootWebScrap.UpdatedDate = DateTime.UtcNow;
                await _webScrapsRepository.SaveWebScrapAsync(rootWebScrap);
            }

            return true;
        }

        private async Task<List<string>> AddWebScrapContent(WebScrap webScrap, string rootUrl)
        {
            _processedWebScraps.Add(new WebScrap { RootUrl = rootUrl, Url = webScrap.Url });
            if (webScrap.LastScrapeDate != null)
            {
                if (webScrap.LastScrapeDate > _rootWebScrapCompletedDate)
                {
                    return webScrap.SubUrls;
                }

                // Load full WebScrap from DB since the _dbWebScraps excludes WebScrapeRuns
                webScrap = await _webScrapsRepository.GetWebScrapByIdAsync(webScrap.Id) ?? webScrap;
            }

            string htmlContent;
            List<string> subUrls = new();
            try
            {
                var webContentProvider = _webContentProviderDelegate(webScrap.MediaType);
                var htmlDoc = await webContentProvider.GetWebContent(webScrap.Url);
                RemoveScripts(htmlDoc);
                htmlContent = htmlDoc.DocumentNode?.OuterHtml ?? "";
                subUrls = ExtractSubUrls(htmlDoc, webScrap.Url, rootUrl);

                Console.WriteLine(_processedWebScraps.Count + $" + {webScrap.Url}");
            }
            catch (Exception e)
            {
                //TODO: Catch unreachable/non-existent host/domain
                var message =
                    $"{webScrap.Url}: Unable to scrape. Check if this is a file url or a redirect to file url. [{e.Message}]";
                htmlContent = message;
            }

            var webScrapeRun = new WebScrapeRun
            {
                Id = Guid.NewGuid().ToString(),
                ScrapeDate = DateTime.UtcNow,
                HtmlContent = htmlContent
            };

            webScrap.SubUrls = subUrls;
            webScrap.WebScrapeRuns.Add(webScrapeRun);
            webScrap.LastScrapeDate = DateTime.UtcNow;

            await _webScrapsRepository.SaveWebScrapAsync(webScrap);

            _scrapeCount++;
            return subUrls;
        }

        // scripts, styles, etc. are heavy contents but not needed
        private static void RemoveScripts(HtmlDocument htmlDoc)
        {
            var scriptNodes = htmlDoc.DocumentNode.DescendantsAndSelf()
                .Where(n => n.Name.Equals("script", StringComparison.InvariantCultureIgnoreCase) ||
                            n.Name.Equals("noscript", StringComparison.InvariantCultureIgnoreCase) ||
                            n.Name.Equals("link", StringComparison.InvariantCultureIgnoreCase) ||
                            n.Name.Equals("style", StringComparison.InvariantCultureIgnoreCase) ||
                            n.Name.Equals("meta", StringComparison.InvariantCultureIgnoreCase) ||
                            n.Name.Equals("svg", StringComparison.InvariantCultureIgnoreCase)
                ).ToArray();

            foreach (var scriptNode in scriptNodes)
            {
                scriptNode.Remove();
            }
        }

        private async Task<WebScrap> RegisterWebScrapUrl(string url, string rootUrl = "", string parentId = "")
        {
            var webScrap = new WebScrap
            {
                Id = Guid.NewGuid().ToString(),
                MediaType = MediaOutletType.Website,
                Url = url,
                RootUrl = rootUrl,
                CreatedDate = DateTime.UtcNow,
                WebScrapeRuns = new List<WebScrapeRun>()
            };

            if (!string.IsNullOrEmpty(parentId))
            {
                webScrap.ParentId = parentId;
            }

            await _webScrapsRepository.SaveWebScrapAsync(webScrap);
            return webScrap;
        }

        private static bool IsValidUrl(string urlString)
        {
            var isValidUrl = Uri.TryCreate(urlString, UriKind.Absolute, out var uriResult)
                             && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return isValidUrl && !IsFileUrl(urlString) && !IsMailToUrl(urlString);
        }

        //File urls are not supported, throws exception from providers
        private static bool IsFileUrl(string urlString)
        {
            // TODO: If video (.mp4) add to videos or web scrap
            char[] excessChars = { '/', '?' };
            var url = urlString.TrimEnd(excessChars);

            string[] fileExtensions =
                { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".zip", ".mp4" /* add more as needed */ };
            return fileExtensions.Any(extension =>
                url.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool IsMailToUrl(string urlString)
        {
            return urlString.IndexOf("email-protection#", StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        private static List<string> ExtractSubUrls(HtmlDocument htmlDoc, string currentUrl, string rootUrl)
        {
            var subUrls = new List<string>();

            var linkNodes = htmlDoc.DocumentNode.SelectNodes("//a[@href]");
            if (linkNodes == null)
            {
                return subUrls;
            }

            foreach (var link in linkNodes)
            {
                var href = link.GetAttributeValue("href", "");

                // Region only in page, same with parent/current URL
                if (href.StartsWith("#"))
                {
                    continue;
                }

                // Construct absolute URL if the href attribute is a relative URL
                var baseUri = new Uri(currentUrl);
                var absoluteUri = new Uri(baseUri, href);
                var absoluteUrl = absoluteUri.AbsoluteUri;

                var rootUrlNonWww = rootUrl.Replace("://www.", "://");

                // Get only subs of the root URL, excludes other urls/domains; remove duplicates
                if ((absoluteUrl.StartsWith(rootUrl, StringComparison.InvariantCultureIgnoreCase) ||
                     absoluteUrl.StartsWith(rootUrlNonWww, StringComparison.InvariantCultureIgnoreCase))
                    && !subUrls.Any(su => su.Equals(absoluteUrl,
                        StringComparison.InvariantCultureIgnoreCase)))
                {
                    subUrls.Add(absoluteUrl);
                }
            }

            var sortedSubUrls = subUrls.OrderBy(su => su).ToList();
            return sortedSubUrls;
        }

        private async Task ScrapeSubUrls(WebScrap currentWebScrap, List<string> subUrls, string rootUrl)
        {
            foreach (var subUrl in subUrls.Where(subUrl =>
                         !IsSubUrlsPerRunLimitReached() && !IsProcessed(subUrl) && IsValidUrl(subUrl)))
            {
                var newWebScrap = await RegisterWebScrapUrl(subUrl, rootUrl, currentWebScrap.Id);

                var webScrapeResult = await AddWebScrapContent(newWebScrap, rootUrl);
                var subSubUrls = webScrapeResult.Select(u => u).ToList();

                await ScrapeSubUrls(newWebScrap, subSubUrls, rootUrl);
            }

            /*// This is fast but results to request being blocked; TODO: revisit
            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = 10
            };
            await Parallel.ForEachAsync(subUrls, parallelOptions, async (subUrl, token) =>
            {
                if (!IsSubUrlsPerRunLimitReached() && !IsProcessed(subUrl) && IsValidUrl(subUrl))
                {
                    var newWebScrap = await RegisterWebScrapUrl(subUrl, rootUrl, currentWebScrap.Id);

                    var webScrapeResult = await AddWebScrapContent(newWebScrap, rootUrl);
                    var subSubUrls = webScrapeResult.Select(u => u).ToList();

                    await ScrapeSubUrls(newWebScrap, subSubUrls, rootUrl);
                }
            });*/
        }

        private bool IsSubUrlsLimitReached()
        {
            return _subUrlsLimit > 0 && _dbWebScraps.Count(dw => dw.LastScrapeDate > _rootWebScrapCompletedDate) >=
                _subUrlsLimit;
        }

        private bool IsSubUrlsPerRunLimitReached()
        {
            return _subUrlsPerRunLimit > 0 && _scrapeCount >= _subUrlsPerRunLimit;
        }

        private bool IsProcessed(string url)
        {
            //Not part of DB webScraps (will be processed separately in main)
            var isInDbWebScraps =
                _dbWebScraps.Any(dw => dw.Url.Equals(url, StringComparison.InvariantCultureIgnoreCase));
            if (isInDbWebScraps)
            {
                return true;
            }

            var isInProcessedWebScraps =
                _processedWebScraps.Any(pw => pw.Url.Equals(url, StringComparison.InvariantCultureIgnoreCase));
            return isInProcessedWebScraps;
        }
    }
}