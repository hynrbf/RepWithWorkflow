using Common.Entities;

namespace Common
{
    public interface ICalendlyService
    {
        Task<CalendlyUserResourceResultK?> GetApplicationAccountAsync();
        Task<CalendlyWebhookResourceResultK?> RegisterWebhookSubscriptionAsync(CalendlyWebhookRequestK webhookRequestK);
        Task<CalendlyWebhookSubscriptionCollectionResultK?> GetWebhookSubscriptionsAsync(string userUrl, string organizationUrl, string scope = "user");
        Task<CalendlyEventTypeCollectionResultK?> GetEventTypesAsync(string userUrl);
        void Register(string token);
    }
}
