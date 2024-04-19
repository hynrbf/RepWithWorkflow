using Newtonsoft.Json;

namespace Common.Entities
{
    public class CalendlyUserK
    {
        [JsonProperty("avatar_url")] public string? AvatarUrl { get; set; }
        [JsonProperty("created_at")] public string? Created { get; set; }
        [JsonProperty("current_organization")] public string? CurrentOrganizationUrl { get; set; }
        [JsonProperty("email")] public string? Email { get; set; }
        [JsonProperty("name")] public string? Name { get; set; }
        [JsonProperty("timezone")] public string? TimeZone { get; set; }
        [JsonProperty("updated_at")] public string? Updated { get; set; }
        [JsonProperty("uri")] public string? UserUrl { get; set; }
        [JsonProperty("resource_type")] public string? ResourceType { get; set; }
        [JsonProperty("scheduling_url")] public string? SchedulingUrl { get; set; }
        [JsonProperty("slug")] public string? Slug { get; set; }
    }
}
