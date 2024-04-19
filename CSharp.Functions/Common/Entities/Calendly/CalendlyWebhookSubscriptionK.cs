using Newtonsoft.Json;

namespace Common.Entities
{
    public class CalendlyWebhookSubscriptionK
    {
        [JsonProperty("callback_url")] public string? CallBackUrl { get; set; }
        [JsonProperty("created_at")] public string? Created { get; set; }
        [JsonProperty("creator")] public string? Creator { get; set; }
        [JsonProperty("user")] public string? UserUrl { get; set; }
        [JsonProperty("organization")] public string? OrganizationUrl { get; set; }
        [JsonProperty("retry_started_at")] public string? RetryStarted { get; set; }
        [JsonProperty("scope")] public string? Scope { get; set; }
        [JsonProperty("state")] public string? State { get; set; }
        [JsonProperty("updated_at")] public string? Updated { get; set; }
        [JsonProperty("uri")] public string? Url { get; set; }
        [JsonProperty("events")] public IEnumerable<string> Events { get; set; } = new List<string>();
    }
}
