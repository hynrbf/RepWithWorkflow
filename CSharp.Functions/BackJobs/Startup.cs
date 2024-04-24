using BackJobs.Infra;
using Common;
using Common.Infra;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

[assembly: FunctionsStartup(typeof(BackJobs.Startup))]

namespace BackJobs
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //register
            builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
            builder.Services.AddSingleton<IAppointedRepresentativeRepository, AppointedRepresentativeRepository>();
            builder.Services.AddSingleton<IOrganizationalStructureRepository, OrganizationalStructureRepository>();
            builder.Services.AddSingleton<IProvidersRepository, ProvidersRepository>();
            builder.Services.AddSingleton<IAffiliatesRepository, AffiliatesRepository>();
            builder.Services.AddSingleton<IIntroducersRepository, IntroducersRepository>();
            builder.Services.AddSingleton<ISaveFieldsRepository, SaveFieldsRepository>();
            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<IHtmlRepository, DocumentHtmlContentRepository>();
            builder.Services.AddSingleton<ISignatureService, SignatureService>();
            builder.Services.AddSingleton<IPdfService, PdfService>();
            builder.Services.AddSingleton<ISettingRepository, SettingRepository>();
            builder.Services.AddSingleton<ICalendlyService, CalendlyService>();
            builder.Services.AddSingleton<IPdfWriterService, PdfWriterService>();
            builder.Services.AddSingleton<IPdfParserService, PdfParserService>();
            builder.Services.AddSingleton<IBlobContainerService, BlobContainerService>();

            //settings
            AppSettingsProvider.Instance.SetAppSettings(() =>
            {
                var listSettings = new Lazy<Dictionary<string, string>>();             
                listSettings.Value.Add(AppConstants.CalendlyBaseApi, "https://api.calendly.com");
                return listSettings;
            });
        }
    }
}