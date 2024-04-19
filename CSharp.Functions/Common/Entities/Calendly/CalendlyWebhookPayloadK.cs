using Newtonsoft.Json;

namespace Common.Entities
{
    public class CalendlyWebhookPayloadK
    {
        [JsonProperty("cancel_url")] public string? cancelUrl { get; set; }
        [JsonProperty("created_at")] public string? Created { get; set; }
        [JsonProperty("email")] public string? Email { get; set; }
        [JsonProperty("event")] public string? EventUrl { get; set; }
        [JsonProperty("first_name")] public string? FirstName { get; set; }
        [JsonProperty("last_name")] public string? LastName { get; set; }
        [JsonProperty("name")] public string? DisplayName { get; set; }
        [JsonProperty("reschedule_url")] public string? RescheduleUrl { get; set; }
        [JsonProperty("rescheduled")] public bool IsRescheduled { get; set; }
        [JsonProperty("status")] public string? Status { get; set; }
        [JsonProperty("timezone")] public string? TimeZone { get; set; }
        [JsonProperty("updated_at")] public string? Updated { get; set; }
        [JsonProperty("uri")] public string? ScheduledEventUrl { get; set; }
        [JsonProperty("scheduled_event")] public CalendlyScheduledEventK? ScheduledEvent { get; set; }
    }
}
