using Newtonsoft.Json;

namespace Common.Entities
{
    public class EmbeddedInviteResponseK
    {
        [JsonProperty("data")] public IEnumerable<FieldInviteK> Data { get; set; } = new List<FieldInviteK>();
    }
}
