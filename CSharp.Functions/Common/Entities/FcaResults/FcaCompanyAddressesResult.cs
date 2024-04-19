using Newtonsoft.Json;

namespace Common.Entities
{
    public class FcaCompanyAddressesResult : FcaCompanyResultBase
    {
        [JsonProperty("Data")] public IEnumerable<FcaAddressDetails>? Addresses { get; set; } = new List<FcaAddressDetails>();
    }
}
