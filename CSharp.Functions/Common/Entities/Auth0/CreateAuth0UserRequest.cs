using Newtonsoft.Json;

namespace Common.Entities
{
    public class CreateAuth0UserRequest
    {
        [JsonProperty("email")] public string? Email { get; set; }
        [JsonProperty("given_name")] public string? FirstName { get; set; }
        [JsonProperty("family_name")] public string? LastName { get; set; }
        [JsonProperty("name")] public string? DisplayName { get; set; }
        [JsonProperty("user_id")] public string? Id { get; set; }
        [JsonProperty("connection")] public string? Connection { get; set; }
        [JsonProperty("password")] public string? Password { get; set; }
    }

    public class OnboardingChangePasswordPayload
    {
        [JsonProperty("email")] public string? Email { get; set; }
        [JsonProperty("password")] public string? Password { get; set; }
        [JsonProperty("onboardingType")] public string? OnboardingType { get; set; }
    }
}