using Newtonsoft.Json;

namespace Common.Entities
{
    public class Signatory
    {
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("role")] public string Role { get; set; }
        [JsonProperty("order")] public int Order { get; set; }
        [JsonProperty("expiration_days")] public int ExpirationDays { get; set; }
        [JsonProperty("subject")] public string Subject { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
    }
}
