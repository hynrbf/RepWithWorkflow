using Newtonsoft.Json;

namespace Common.Entities
{
    public class FcaCompanySearchResult : FcaCompanyResultBase
    {
        [JsonProperty("Data")] public IEnumerable<FcaCompanyK>? FcaCompanies { get; set; }
    }
}
