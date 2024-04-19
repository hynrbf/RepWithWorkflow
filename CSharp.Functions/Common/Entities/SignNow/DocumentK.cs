using Newtonsoft.Json;

namespace Common.Entities
{
    public class DocumentK
    {
        [JsonProperty("id")] public string? Id { get; set; }
        [JsonProperty("user_id")] public string? UserId { get; set; }
        [JsonProperty("document_name")] public string? DocumentName { get; set; }
        [JsonProperty("page_count")] public string? PageCount { get; set; }
        [JsonProperty("created")] public string? Created { get; set; }
        [JsonProperty("updated")] public string? Updated { get; set; }
        [JsonProperty("original_filename")] public string? OriginalFileName { get; set; }
        [JsonProperty("owner")] public string? Owner { get; set; }
        [JsonProperty("roles")] public IEnumerable<RoleK> Roles { get; set; } = new List<RoleK>();
        [JsonProperty("field_invites")] public IEnumerable<FieldInviteK> FieldInvites { get; set; } = new List<FieldInviteK>();
    }
}
