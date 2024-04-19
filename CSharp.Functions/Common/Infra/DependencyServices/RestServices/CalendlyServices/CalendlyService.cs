using Common.Entities;
using Newtonsoft.Json;
using System.Text;

namespace Common.Infra
{
    public class CalendlyService : ICalendlyService
    {
        private string? _personalAccessToken;
        private static string CalendlyBaseUrl => AppSettingsProvider.Instance.GetValue(AppConstants.CalendlyBaseApi);

        public async Task<CalendlyUserResourceResultK?> GetApplicationAccountAsync()
        {
            var endpoint = $"{CalendlyBaseUrl}/users/me";
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Add("Authorization", $"Bearer {_personalAccessToken}");
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            var userResource = JsonConvert.DeserializeObject<CalendlyUserResourceResultK>(result);
            return userResource;
        }

        public async Task<CalendlyWebhookSubscriptionCollectionResultK?> GetWebhookSubscriptionsAsync(string userUrl, string organizationUrl, string scope)
        {
            var endpoint = $"{CalendlyBaseUrl}/webhook_subscriptions?user={userUrl}&organization={organizationUrl}&scope={scope}";
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Add("Authorization", $"Bearer {_personalAccessToken}");
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            var webhookSubscription = JsonConvert.DeserializeObject<CalendlyWebhookSubscriptionCollectionResultK>(result);
            return webhookSubscription;
        }

        public async Task<CalendlyWebhookResourceResultK?> RegisterWebhookSubscriptionAsync(CalendlyWebhookRequestK webhookRequestK)
        {
            var endpoint = $"{CalendlyBaseUrl}/webhook_subscriptions";
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add("Authorization", $"Bearer {_personalAccessToken}");
            var jsonContent = JsonConvert.SerializeObject(webhookRequestK);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            var webhookSubscription = JsonConvert.DeserializeObject<CalendlyWebhookResourceResultK>(result);
            return webhookSubscription;
        }

        public async Task<CalendlyEventTypeCollectionResultK?> GetEventTypesAsync(string userUrl)
        {
            var endpoint = $"{CalendlyBaseUrl}/event_types?user={userUrl}";
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Add("Authorization", $"Bearer {_personalAccessToken}");
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            var eventTypes = JsonConvert.DeserializeObject<CalendlyEventTypeCollectionResultK>(result);
            return eventTypes;
        }

        public void Register(string token)
            => _personalAccessToken = token;
    }
}