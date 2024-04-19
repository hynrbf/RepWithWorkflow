using Common.Infra;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Api.Infra;
using Common;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Api.Startup))]

namespace Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //register
            builder.Services.AddSingleton<IGuideSchemaAndUiRepository, GuideSchemaAndUiRepository>();
            builder.Services.AddSingleton<ISchemaRepository, SchemaRepository>();
            builder.Services.AddSingleton<IUiSchemaRepository, UiSchemaRepository>();
            builder.Services.AddSingleton<ISchemaAnswerRepository, SchemaAnswerRepository>();
            builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
            builder.Services.AddSingleton<IHtmlRepository, DocumentHtmlContentRepository>();
            builder.Services.AddSingleton<ISignatureService, SignatureService>();
            builder.Services.AddSingleton<IPdfService, PdfService>();
            builder.Services.AddSingleton<IWordToHtmlService, WordToHtmlService>();
            builder.Services.AddSingleton<IFcaPermissionsRepository, FcaPermissionsRepository>();
            builder.Services.AddSingleton<IFcaStatusRepository, FcaStatusRepository>();
            builder.Services.AddSingleton<IEmailQueueRepository, EmailQueueRepository>();
            builder.Services.AddSingleton<ICompaniesHouseService, CompaniesHouseService>();
            builder.Services.AddSingleton<IFcaService, FcaService>();
            builder.Services.AddSingleton<IIcoService, IcoService>();
            builder.Services.AddSingleton<IRatingsService, RatingsService>();
            builder.Services.AddSingleton<IAuth0Service, Auth0Service>();
            builder.Services.AddSingleton<ISettingRepository, SettingRepository>();
            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<IMeetingRequestRepository, MeetingRequestRepository>();
            builder.Services.AddSingleton<IDocGeneratorService, DocGeneratorService>();
            builder.Services.AddSingleton<IBlobContainerService, BlobContainerService>();
            builder.Services.AddSingleton<ICalendlyService, CalendlyService>();
            builder.Services.AddSingleton<IGetAddressService, GetAddressService>();
            builder.Services.AddSingleton<IFileUploaderService, FileUploaderService>();
            builder.Services.AddSingleton<ICommentRepository, CommentRepository>();
            builder.Services.AddSingleton<IWebContentsScrapping, WebContentsScrapping>();
            builder.Services.AddSingleton<IOrganizationalStructureRepository, OrganizationalStructureRepository>();
            builder.Services.AddSingleton<IProvidersRepository, ProvidersRepository>();
            builder.Services.AddSingleton<IIntroducersRepository, IntroducersRepository>();
            builder.Services.AddSingleton<IWebScrapsRepository, WebScrapsRepository>();
            builder.Services.AddSingleton<IFinancialPromotionRepository, FinancialPromotionRepository>();
            builder.Services.AddSingleton<ICsvLookupService, CsvLookupService>();
            builder.Services.AddSingleton<IFileDownloaderService, FileDownloaderService>();
            builder.Services.AddSingleton<IWebScrapeService, WebScrapeService>();
            builder.Services.AddSingleton<ICurrencyConversionService, CurrencyConversionService>();
            builder.Services.AddSingleton<ICurrencyConversionRepository, CurrencyConversionRepository>();
            builder.Services.AddSingleton<IAppointedRepresentativeRepository, AppointedRepresentativeRepository>();
            builder.Services.AddSingleton<IBaseFirmPermissionRepository, BaseFirmPermissionRepository>();
            builder.Services.AddSingleton<IFcaRoleRepository, FcaRoleRepository>();
            builder.Services.AddSingleton<ICompanyHouseRepository, CompanyHouseRepository>();
            builder.Services.AddSingleton<IProductMappingRepository, ProductMappingRepository>();
            builder.Services.AddSingleton<ICustomerReplicationRepository, CustomerReplicationRepository>();
            builder.Services.AddSingleton<ICustomerFcaReplicationRepository, CustomerFcaReplicationRepository>();

            //settings
            AppSettingsProvider.Instance.SetAppSettings(() =>
            {
                var listSettings = new Lazy<Dictionary<string, string>>();
                listSettings.Value.Add(AppConstants.RestBaseApi, "https://default.com/api");
                listSettings.Value.Add(AppConstants.RestBaseCompanyHouseApi,
                    "https://api.company-information.service.gov.uk");
                listSettings.Value.Add(AppConstants.RestBaseFcaApi, "https://register.fca.org.uk");
                listSettings.Value.Add(AppConstants.RestBaseIcoApi, "https://ico.org.uk");
                listSettings.Value.Add(AppConstants.CalendlyBaseApi, "https://api.calendly.com");
                listSettings.Value.Add(AppConstants.GetAddressBaseApi, "https://api.getAddress.io");
                listSettings.Value.Add(AppConstants.RestBaseFixerApi, "http://data.fixer.io/api/");
                listSettings.Value.Add(AppConstants.InsurerRatingsApi,
                    "https://richdale.azurewebsites.net/api/ratings");
                return listSettings;
            });
        }
    }
}