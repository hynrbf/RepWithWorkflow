using Newtonsoft.Json;

namespace Common.Entities
{
    public class CalendlyProfileK
    {
        [JsonProperty("name")] public string? Name { get; set; }
        [JsonProperty("owner")] public string? OwnerUrl { get; set; }
        [JsonProperty("type")] public string? Type { get; set; }
    }
}
