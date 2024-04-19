using Common;
using Common.Infra;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(BackJobsChangeHistory.Startup))]

namespace BackJobsChangeHistory
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //register
            builder.Services.AddSingleton<ICustomerReplicationRepository, CustomerReplicationRepository>();
            builder.Services.AddSingleton<ICustomerFcaReplicationRepository, CustomerFcaReplicationRepository>();
        }
    }
}