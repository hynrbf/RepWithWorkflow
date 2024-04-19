using Common;
using Common.Infra;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(BackJobsDocumentGenerator.Startup))]

namespace BackJobsDocumentGenerator
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //register
            builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
            builder.Services.AddSingleton<IHtmlRepository, DocumentHtmlContentRepository>();
            builder.Services.AddSingleton<ISettingRepository, SettingRepository>();
            builder.Services.AddSingleton<IDocGeneratorService, DocGeneratorService>();
            builder.Services.AddSingleton<IBlobContainerService, BlobContainerService>();
        }
    }
}