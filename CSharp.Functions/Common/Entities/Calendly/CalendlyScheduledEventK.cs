using Newtonsoft.Json;

namespace Common.Entities
{
    public class CalendlyScheduledEventK
    {
        [JsonProperty("created_at")] public DateTime Created { get; set; }
        [JsonProperty("start_time")] public DateTime StartTime { get; set; }
        [JsonProperty("end_time")] public DateTime EndTime { get; set; }
        [JsonProperty("event_type")] public string? EventTypeUrl { get; set; }
        [JsonProperty("name")] public string? Name { get; set; }
        [JsonProperty("status")] public string? Status { get; set; }
        [JsonProperty("updated_at")] public string? Updated { get; set; }
        [JsonProperty("uri")] public string? Url { get; set; }
        [JsonProperty("event_memberships")] public IEnumerable<CalendlyEventMembershipK> EventMemberships { get; set; } = new List<CalendlyEventMembershipK>();
        [JsonProperty("location")] public CalendlyLocationK? Location { get; set; }
    }
}
