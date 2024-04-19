using Common.Infra;
using Newtonsoft.Json;

namespace Common.Entities
{
    public class GetAddressFilterWithResultCount
    {
        [JsonProperty("filter")] public GetAddressFilter Filter { get; set; }
        [JsonProperty("top")] public int ResultCount { get; set; } = AppConstants.DefaultItemsPerPageNoUsedInFcaOrCh;    // max
    }
}
