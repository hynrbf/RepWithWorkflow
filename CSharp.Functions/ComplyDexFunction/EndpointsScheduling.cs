using Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public partial class Endpoints
    {
        //Calendly Services
        // not used in frontend just testing in postman
        [FunctionName(nameof(GetApplicationAccountAsync))]
        public async Task<IActionResult> GetApplicationAccountAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var applicationUserAccount = await _calendlyService.GetApplicationAccountAsync();
            return new OkObjectResult(applicationUserAccount);
        }

        // not used in frontend just testing in postman
        [FunctionName(nameof(GetWebhookSubscriptionsAsync))]
        public async Task<IActionResult> GetWebhookSubscriptionsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var userUrl = req.Query["user"];
            var organizationUrl = req.Query["organization"];
            var scope = req.Query["scope"];
            var webhookSubscriptions =
                await _calendlyService.GetWebhookSubscriptionsAsync(userUrl, organizationUrl, scope);
            return new OkObjectResult(webhookSubscriptions);
        }

        // not used in frontend just testing in postman
        [FunctionName(nameof(RegisterWebhookSubscriptionAsync))]
        public async Task<IActionResult> RegisterWebhookSubscriptionAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var webhookRequest = JsonConvert.DeserializeObject<CalendlyWebhookRequestK>(requestBody);
            var webhookSubscription = await _calendlyService.RegisterWebhookSubscriptionAsync(webhookRequest);
            return new OkObjectResult(webhookSubscription);
        }

        [FunctionName(nameof(GetCalendlyEventTypesAsync))]
        public async Task<IActionResult> GetCalendlyEventTypesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var applicationUserAccount = await _calendlyService.GetApplicationAccountAsync();

            if (string.IsNullOrEmpty(applicationUserAccount?.Resource?.UserUrl))
            {
                return new OkResult();
            }

            var eventTypes = await _calendlyService.GetEventTypesAsync(applicationUserAccount.Resource.UserUrl);

            //check if we registered subscription
            var userUrl = applicationUserAccount.Resource.UserUrl;
            var orgUrl = applicationUserAccount.Resource.CurrentOrganizationUrl;
            var existingSubscription = await _calendlyService.GetWebhookSubscriptionsAsync(userUrl,
                orgUrl ?? throw new NullReferenceException("Organization url not found in calendly service"));

            if (existingSubscription == null)
            {
                throw new NullReferenceException("Cannot continue because you dont have existing subscription");
            }

            if (existingSubscription.Collection.Any())
            {
                return new OkObjectResult(eventTypes);
            }

            var callbackBaseUrl =
                Environment.GetEnvironmentVariable("CalendlyCallbackUrlHostName", EnvironmentVariableTarget.Process);
            var callback = $"https://{callbackBaseUrl}/api/CalendlyCallbackAsync";
            // Subscribe to both created and canceled
            await _calendlyService.RegisterWebhookSubscriptionAsync(new CalendlyWebhookRequestK
            {
                CallbackUrl = callback,
                Events = new List<string> { "invitee.created", "invitee.canceled" },
                UserUrl = userUrl,
                OrganizationUrl = orgUrl,
                Scope = "user"
            });
            return new OkObjectResult(eventTypes);
        }
    }
}