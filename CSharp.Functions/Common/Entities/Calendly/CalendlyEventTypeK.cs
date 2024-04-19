using Newtonsoft.Json;

namespace Common.Entities
{
    public class CalendlyEventTypeK
    {
        [JsonProperty("active")] public bool Active { get; set; }
        [JsonProperty("booking_method")] public string? BookingMethod { get; set; }
        [JsonProperty("created_at")] public string? Created { get; set; }
        [JsonProperty("duration")] public int Duration { get; set; }
        [JsonProperty("kind")] public string? Kind { get; set; }
        [JsonProperty("name")] public string? Name { get; set; }
        [JsonProperty("scheduling_url")] public string? SchedulingUrl { get; set; }
        [JsonProperty("slug")] public string? Slug { get; set; }
        [JsonProperty("type")] public string? Type { get; set; }
        [JsonProperty("updated_at")] public string? Updated { get; set; }
        [JsonProperty("uri")] public string? Url { get; set; }
        [JsonProperty("description_plain")] public string? Description { get; set; }
        [JsonProperty("description_html")] public string? DescriptionHtml { get; set; }
        [JsonProperty("profile")] public CalendlyProfileK Profile { get; set; }
    }
}
