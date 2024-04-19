using Common;
using Common.Infra;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System;
using BackJobsDataInitializer.Infra;

[assembly: FunctionsStartup(typeof(BackJobsDataInitializer.Startup))]

namespace BackJobsDataInitializer
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //register
            builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
            builder.Services.AddSingleton<ISettingRepository, SettingRepository>();
            builder.Services.AddSingleton<ICompaniesHouseService, CompaniesHouseService>();
            builder.Services.AddSingleton<IFcaService, FcaService>();
            builder.Services.AddSingleton<IIcoService, IcoService>();
            builder.Services.AddSingleton<ICustomerDataInitService, CustomerDataInitService>();
            builder.Services.AddSingleton<IWebContentsScrapping, WebContentsScrapping>();
            builder.Services.AddSingleton<IBlobContainerService, BlobContainerService>();
            builder.Services.AddSingleton<IOrganizationalStructureRepository, OrganizationalStructureRepository>();
            builder.Services.AddSingleton<IWebScrapsRepository, WebScrapsRepository>();
            builder.Services.AddSingleton<IFinancialPromotionRepository, FinancialPromotionRepository>();
            builder.Services.AddSingleton<IWebScrapeService, WebScrapeService>();
            builder.Services.AddSingleton<IFcaPermissionsRepository, FcaPermissionsRepository>();
            builder.Services.AddSingleton<IAppointedRepresentativeRepository, AppointedRepresentativeRepository>();
            builder.Services.AddSingleton<IFcaRoleRepository, FcaRoleRepository>();

            //settings
            AppSettingsProvider.Instance.SetAppSettings(() =>
            {
                var listSettings = new Lazy<Dictionary<string, string>>();
                listSettings.Value.Add(AppConstants.RestBaseCompanyHouseApi,
                    "https://api.company-information.service.gov.uk");
                listSettings.Value.Add(AppConstants.RestBaseFcaApi, "https://register.fca.org.uk");
                listSettings.Value.Add(AppConstants.RestBaseIcoApi, "https://ico.org.uk");
                return listSettings;
            });
        }
    }
}