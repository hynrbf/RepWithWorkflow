using Newtonsoft.Json;

namespace Common.Entities
{
    public class CalendlyWebhookRequestK
    {
        [JsonProperty("url")] public string? CallbackUrl { get; set; }
        [JsonProperty("organization")] public string? OrganizationUrl { get; set; }
        [JsonProperty("user")] public string? UserUrl { get; set; }
        [JsonProperty("scope")] public string? Scope { get; set; }
        [JsonProperty("events")] public IEnumerable<string?> Events { get; set; } = new List<string>();
    }
}
