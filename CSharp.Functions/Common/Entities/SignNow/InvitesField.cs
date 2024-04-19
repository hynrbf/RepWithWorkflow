using Newtonsoft.Json;

namespace Common.Entities
{
    public class InvitesField
    {
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
    }
}
