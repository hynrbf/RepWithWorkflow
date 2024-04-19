using Newtonsoft.Json;

namespace Common.Entities
{
    public class FcaFirmName
    {
        [JsonProperty("Effective To")] public string? EffectiveTo { get; set; }
        [JsonProperty("Effective From")] public string? EffectiveFrom { get; set; }
        public string? Status { get; set; }
        public string? Name { get; set; }
    }
}
