using Newtonsoft.Json;

namespace Common.Entities
{
    public class CalendlyLocationK
    {
        [JsonProperty("location")] public string? Detail { get; set; }
        [JsonProperty("type")] public string? Type { get; set; }
    }
}
