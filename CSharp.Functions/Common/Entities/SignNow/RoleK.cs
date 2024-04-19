using Newtonsoft.Json;

namespace Common.Entities
{
    public class RoleK
    {
        [JsonProperty("unique_id")] public string? Id { get; set; }
        [JsonProperty("signing_order")] public string? SigningOrder { get; set; }
        [JsonProperty("name")] public string? Name { get; set; }
    }
}
