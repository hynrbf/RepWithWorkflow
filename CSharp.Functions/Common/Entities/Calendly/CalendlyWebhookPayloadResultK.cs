using Newtonsoft.Json;

namespace Common.Entities
{
    public class CalendlyWebhookPayloadResultK
    {
        [JsonProperty("created_at")] public string? Created { get; set; }
        [JsonProperty("created_by")] public string? CreatedBy { get; set; }
        [JsonProperty("event")] public string? Event { get; set; }
        [JsonProperty("payload")] public CalendlyWebhookPayloadK? Payload { get; set; }
    }
}
