using Newtonsoft.Json;

namespace Common.Entities
{
    public class FcaCompanyDetailsResult : FcaCompanyResultBase
    {
        [JsonProperty("Data")] public IEnumerable<FcaFirmDetail>? Detail { get; set; }
    }
}
