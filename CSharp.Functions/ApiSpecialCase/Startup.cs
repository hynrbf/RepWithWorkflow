using Common;
using Common.Infra;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ApiSpecialCase.Startup))]
namespace ApiSpecialCase
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //register
            builder.Services.AddSingleton<IMeetingRequestRepository, MeetingRequestRepository>();
        }
    }
}
