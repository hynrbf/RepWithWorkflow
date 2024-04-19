using Newtonsoft.Json;

namespace Common.Entities
{
    public class InviteK
    {
        [JsonProperty("email")] public string? Email { get; set; }
        [JsonProperty("role_id")] public string? RoleId { get; set; }
        [JsonProperty("order")] public int Order { get; set; }
        [JsonProperty("auth_method")] public string? AuthMethod { get; set; }
        [JsonProperty("first_name")] public string? FirstName { get; set; }
        [JsonProperty("last_name")] public string? LastName { get; set; }
    }
}
