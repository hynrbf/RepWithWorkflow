using Newtonsoft.Json;

namespace Common.Entities
{
    public class GetAddressFilter
    {
        [JsonProperty("county")] public string? County { get; set; }
        [JsonProperty("country")] public string? Country { get; set; }
        [JsonProperty("locality")] public string? Locality { get; set; }
        [JsonProperty("district")] public string? District { get; set; }
        [JsonProperty("town_or_city")] public string? TownOrCity { get; set; }
        [JsonProperty("postcode")] public string? Postcode { get; set; }
    }
}