using Newtonsoft.Json;

namespace Common.Entities
{
    public class EditDocumentPayload
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("fields")] public List<DocumentField> Fields { get; set; } = new List<DocumentField>();
        [JsonProperty("texts")] public List<TextField> Texts { get; set; } = new List<TextField>();
        [JsonProperty("field_invites")] public List<InvitesField> Invites { get; set; } = new List<InvitesField>();
    }
}