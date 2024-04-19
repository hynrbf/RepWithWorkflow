using Newtonsoft.Json;

namespace Common.Entities
{
    public class CalendlyWebhookSubscriptionCollectionResultK
    {
        [JsonProperty("collection")] public IEnumerable<CalendlyWebhookSubscriptionK> Collection { get; set; } = new List<CalendlyWebhookSubscriptionK>();
    }
}
