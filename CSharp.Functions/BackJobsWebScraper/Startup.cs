using BackJobsWebScaper;
using Common;
using Common.Infra;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System;
using BackJobsWebScraper.Infra;

[assembly: FunctionsStartup(typeof(Startup))]

namespace BackJobsWebScaper
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IWebScrapeService, WebScrapeService>();
            builder.Services.AddSingleton<IWebScrapFinancialPromotionService, WebScrapFinancialPromotionService>();
            builder.Services.AddSingleton<IWebContentProvider, PuppeteerWebContent>();
            builder.Services.AddSingleton<IFinancialPromotionRepository, FinancialPromotionRepository>();
            builder.Services.AddSingleton<IWebScrapsRepository, WebScrapsRepository>();
            builder.Services.AddSingleton<IWebContentsScrapping, WebContentsScrapping>();
            builder.Services.AddSingleton<IBlobContainerService, BlobContainerService>();
            builder.Services.AddSingleton<IFileDownloaderService, FileDownloaderService>();
            builder.Services.AddSingleton<ICsvLookupService, CsvLookupService>();
            builder.Services.AddSingleton<IWebContentParser, WebContentParser>();
            builder.Services.AddSingleton<IServiceMapper, ServiceMapper>();

            AppSettingsProvider.Instance.SetAppSettings(() =>
            {
                var listSettings = new Lazy<Dictionary<string, string>>();
                listSettings.Value.Add(AppConstants.PuppeteerScraperApi, "https://scraper-sun.azurewebsites.net");
                return listSettings;
            });
        }
    }
}