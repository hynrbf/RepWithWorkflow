using Newtonsoft.Json;

namespace Common.Entities
{
    public class FcaCompanyNamesResult : FcaCompanyResultBase
    {
        public IEnumerable<Data> Data { get; set; } = new List<Data>();
    }

    public class Data
    {
        [JsonProperty("Current Names")] public IEnumerable<FcaFirmName> CurrentNames { get; set; } = new List<FcaFirmName>();
        [JsonProperty("Previous Names")] public IEnumerable<FcaFirmName> PreviousNames { get; set; } = new List<FcaFirmName>();
    }
}
