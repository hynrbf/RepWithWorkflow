using Newtonsoft.Json;

namespace Common.Entities
{
    public class CalendlyEventMembershipK
    {
        [JsonProperty("user")] public string? User { get; set; }
        [JsonProperty("user_email")] public string? Email { get; set; }
    }
}
