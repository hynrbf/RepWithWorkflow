using Newtonsoft.Json;

namespace Common.Entities
{
    public class GetAddressItem
    {
        [JsonProperty("id")] public string? Id { get; set; }
        [JsonProperty("address")] public string? Address { get; set; }
        [JsonProperty("url")] public string? Url { get; set; }

    }
}
