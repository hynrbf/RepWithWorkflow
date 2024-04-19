using Newtonsoft.Json;

namespace Common.Entities
{
    public class FcaRegulatorsResult : FcaCompanyResultBase
    {
        public List<FcaRegulator> Data { get; set; } = new();
    }

    public class FcaRegulator
    {
        [JsonProperty("Regulator Name")] public string? Name { get; set; }
        [JsonProperty("Termination Date")] public string? TerminationDate { get; set; }
        [JsonProperty("Effective Date")] public string? EffectiveDate { get; set; }
    }
}