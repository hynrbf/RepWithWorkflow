using Newtonsoft.Json;

namespace Common.Entities
{
    public class CalendlyUserResourceResultK
    {
        [JsonProperty("resource")] public CalendlyUserK? Resource { get; set; }
    }
}
