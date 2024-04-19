using Newtonsoft.Json;

namespace Common.Entities
{
    public class Auth0User
    {
        [JsonProperty("created_at")] public string? Created { get; set; }
        [JsonProperty("email")] public string? Email { get; set; }
        [JsonProperty("email_verified")] public bool IsEmailVerified { get; set; }
        [JsonProperty("family_name")] public string? LastName { get; set; }
        [JsonProperty("given_name")] public string? FirstName { get; set; }
        [JsonProperty("name")] public string? DisplayName { get; set; }
        [JsonProperty("nickname")] public string? NickName { get; set; }
        [JsonProperty("picture")] public string? PictureUrl { get; set; }
        [JsonProperty("updated_at")] public string? Updated { get; set; }
        [JsonProperty("user_id")] public string? Id { get; set; }
        [JsonProperty("last_password_reset")] public string? LastPasswordReset { get; set; }
        [JsonProperty("last_ip")] public string? LastIp { get; set; }
        [JsonProperty("last_login")] public string? LastLogin { get; set; }
        [JsonProperty("logins_count")] public int LoginCount { get; set; }

        [JsonProperty("identities")]
        public IEnumerable<Auth0UserIdentity> Identities { get; set; } = new List<Auth0UserIdentity>();
    }
}