using Newtonsoft.Json;

namespace Common.Entities
{
    public class MeetingRequests
    {
        [JsonProperty("id")] public string Id { get; set; }
        public string? EventUrl { get; set; }
        public string? EventTypeId { get; set; }
        public string? Email { get; set; } //original email, mapped to Customer container
        public string? EmailUsedForCalendly { get; set; }
        public string? DisplayName { get; set; }
        public string? EventName { get; set; }
        public long StartInEpoch { get; set; }
        public long EndInEpoch { get; set; }
        public string? TimeZone { get; set; }
        public string? Status { get; set; }
        public string? Created { get; set; }
        public string? CancelUrl { get; set; }
        public string? LocationOrContactNo { get; set; }
        public string? LocationType { get; set; }
    }
}
