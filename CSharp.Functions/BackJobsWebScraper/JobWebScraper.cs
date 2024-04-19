using BackJobsWebScraper.Infra;
using Common;
using Common.Entities;
using Common.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BackJobsWebScraper
{
    public class JobWebScraper
    {
        private readonly IWebScrapsRepository _webScrapsRepository;

        private readonly IWebScrapeService _webScrapeService;
        private readonly IWebScrapFinancialPromotionService _webScrapFinancialPromotionService;
        private readonly ICsvLookupService _csvLookupService;

        private static readonly SemaphoreSlim Semaphore = new(2, 2);
        private static bool _isAllWebScrapeTasksCompleted = true;
        private static bool _isAllWebScrapToFpTasksCompleted = true;

        public JobWebScraper(IWebScrapsRepository webScrapsRepository, IWebScrapeService webScrapeService,
            IWebScrapFinancialPromotionService webScrapFinancialPromotionService, ICsvLookupService csvLookupService,
            IBlobContainerService blobContainerClientService)
        {
            _webScrapsRepository = webScrapsRepository;
            _webScrapeService = webScrapeService;
            _webScrapFinancialPromotionService = webScrapFinancialPromotionService;
            _csvLookupService = csvLookupService;

            var storageConnection =
                Environment.GetEnvironmentVariable("AzureStorageConnectionString", EnvironmentVariableTarget.Process) ??
                throw new NullReferenceException("AzureStorageConnectionString is null");
            var containerName =
                Environment.GetEnvironmentVariable("BlobStorageContainerName", EnvironmentVariableTarget.Process) ??
                throw new NullReferenceException("BlobStorageContainerName is null");
            blobContainerClientService.Register(storageConnection, containerName);
        }

        [FunctionName("JobWebScraper")]
        public async Task RunWebScraper([TimerTrigger("%WebScraperSchedule%")] TimerInfo myTimer, ILogger log)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
            log.LogInformation("JobWebScraper function executed at {Now} with version {Version}", DateTime.Now,
                version);

            var isWebScraperEnabledValue =
                Environment.GetEnvironmentVariable("IsWebScraperEnabled", EnvironmentVariableTarget.Process);
            if (!bool.TryParse(isWebScraperEnabledValue, out var isWebScraperEnabled))
            {
                isWebScraperEnabled = false;
            }

            if (!isWebScraperEnabled)
            {
                log.LogWarning($"{nameof(RunWebScraper)}: Web Scraper is DISABLED.");
                return;
            }

            await Semaphore.WaitAsync();
            if (!_isAllWebScrapeTasksCompleted)
            {
                Semaphore.Release();
                log.LogInformation("we will run again in the next round when all tasks are completed");
                return;
            }

            _isAllWebScrapeTasksCompleted = false;
            Semaphore.Release();
            var taskStatus = false;

            try
            {
                var scrapeIntervalValue =
                    Environment.GetEnvironmentVariable("ScrapeIntervalInDays", EnvironmentVariableTarget.Process);
                if (!int.TryParse(scrapeIntervalValue, out var scrapeInterval))
                {
                    scrapeInterval = 7;
                }

                var subUrlsLimitValue =
                    Environment.GetEnvironmentVariable("SubUrlsLimit", EnvironmentVariableTarget.Process);
                if (!int.TryParse(subUrlsLimitValue, out var subUrlsLimit))
                {
                    subUrlsLimit = 20;
                }

                var subUrlsPerRunLimitValue =
                    Environment.GetEnvironmentVariable("SubUrlsPerRunLimit", EnvironmentVariableTarget.Process);
                if (!int.TryParse(subUrlsPerRunLimitValue, out var subUrlsPerRunLimit))
                {
                    subUrlsPerRunLimit = 20;
                }

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                await _webScrapeService.RunWebScrape(scrapeInterval, subUrlsLimit, subUrlsPerRunLimit,
                    WebContentProviderDelegate);
                taskStatus = true;

                stopWatch.Stop();
                Console.WriteLine($"Finished in {stopWatch.ElapsedMilliseconds} milliseconds");
            }
            finally
            {
                await Semaphore.WaitAsync();
                _isAllWebScrapeTasksCompleted = taskStatus;
                Semaphore.Release();
            }
        }

        [FunctionName("JobWebScrapToFinancialPromotion")]
        public async Task RunWebScrapToFinancialPromotion([TimerTrigger("%WebScrapToFpSchedule%")] TimerInfo myTimer,
            ILogger log)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
            log.LogInformation("JobWebScraper function executed at {Now} with version {Version}", DateTime.Now,
                version);

            var isWebScrapToFpEnabledSettings =
                Environment.GetEnvironmentVariable("IsWebScrapToFpEnabled", EnvironmentVariableTarget.Process);
            if (!bool.TryParse(isWebScrapToFpEnabledSettings, out var isWebScrapToFpEnabled))
            {
                isWebScrapToFpEnabled = false;
            }

            if (!isWebScrapToFpEnabled)
            {
                log.LogWarning(
                    $"{nameof(RunWebScrapToFinancialPromotion)}: Web Scrape to Financial Promotion is DISABLED.");
                return;
            }

            await Semaphore.WaitAsync();
            if (!_isAllWebScrapToFpTasksCompleted)
            {
                Semaphore.Release();
                log.LogInformation("we will run again in the next round when all tasks are completed");
                return;
            }

            _isAllWebScrapToFpTasksCompleted = false;
            Semaphore.Release();
            var taskStatus = false;

            try
            {
                const string lookupFilename = "DocStore/LookupFiles/Crypto_Mapping.csv";
                var namesForWarning = (await _csvLookupService.GetCryptoNamesAsync("N", lookupFilename))
                    .Select(cn => cn.Name.ToLower()).ToList();

                await _webScrapFinancialPromotionService.RunWebScrapToFinancialPromotion(namesForWarning);
                taskStatus = true;
            }
            finally
            {
                await Semaphore.WaitAsync();
                _isAllWebScrapToFpTasksCompleted = taskStatus;
                Semaphore.Release();
            }
        }

        [FunctionName("CleanUp")]
        public async Task<IActionResult> CleanUp(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            await _webScrapsRepository.DeleteAllWebScrapsAsync();
            return new OkObjectResult("Success!");
        }

        private static IWebContentProvider WebContentProviderDelegate(MediaOutletType mediaOutletType)
        {
            return mediaOutletType == MediaOutletType.Website
                ? new HtmlAgilityPackWebContent()
                : new PuppeteerWebContent();
        }
    }
}
