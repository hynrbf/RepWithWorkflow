using Newtonsoft.Json;

namespace Common.Entities
{
    public class EmbeddedInviteK
    {
        [JsonProperty("name_formula")] public string? NameFormula { get; set; }
        [JsonProperty("invites")] public IEnumerable<InviteK> Invites { get; set; } = new List<InviteK>();
    }
}
