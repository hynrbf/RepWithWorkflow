using Newtonsoft.Json;

namespace Common.Entities
{
    public class GetAddressSuggestion
    {
        [JsonProperty("suggestions")] public List<GetAddressItem> Suggestions { get; set; } = new List<GetAddressItem>();
    }
}
