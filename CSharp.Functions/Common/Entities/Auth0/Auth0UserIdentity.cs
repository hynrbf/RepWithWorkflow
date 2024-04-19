using Newtonsoft.Json;

namespace Common.Entities
{
    public class Auth0UserIdentity
    {
        [JsonProperty("user_id")] public string? Id { get; set; }
        [JsonProperty("connection")] public string? Connection { get; set; }
        [JsonProperty("provider")] public string? Provider { get; set; }
        [JsonProperty("isSocial")] public bool IsSocial { get; set; }
    }
}