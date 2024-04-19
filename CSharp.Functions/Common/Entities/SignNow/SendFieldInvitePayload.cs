using Newtonsoft.Json;

namespace Common.Entities
{
    public class SendFieldInvitePayload
    {
        [JsonProperty("document_id")] public string DocumentId { get; set; }
        [JsonProperty("to")] public List<Signatory> To { get; set; } = new();
        [JsonProperty("from")] public string From { get; set; }
    }
}