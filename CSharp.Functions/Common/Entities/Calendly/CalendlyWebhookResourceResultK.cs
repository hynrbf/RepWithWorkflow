using Newtonsoft.Json;

namespace Common.Entities
{
    public class CalendlyWebhookResourceResultK
    {
        [JsonProperty("resource")] public CalendlyWebhookSubscriptionK? Resource { get; set; }
    }
}
