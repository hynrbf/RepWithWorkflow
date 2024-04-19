using Newtonsoft.Json;

namespace Common.Entities
{
    public class FieldInviteK
    {
        [JsonProperty("id")] public string? Id { get; set; }
        [JsonProperty("signer_user_id")] public string? SignerUserId { get; set; }
        [JsonProperty("status")] public string? Status { get; set; }
        [JsonProperty("created")] public string? Created { get; set; }
        [JsonProperty("role")] public string? Role { get; set; }
        [JsonProperty("updated")] public string? Updated { get; set; }
        [JsonProperty("expiration_time")] public string? ExpirationTime { get; set; }
        [JsonProperty("email")] public string? Email { get; set; }
        [JsonProperty("role_id")] public string? RoleId { get; set; }
        [JsonProperty("order")] public int Order { get; set; }
    }
}
