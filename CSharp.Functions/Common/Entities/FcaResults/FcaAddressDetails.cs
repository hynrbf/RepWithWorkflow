using Newtonsoft.Json;

namespace Common.Entities
{
    public class FcaAddressDetails
    {
        public string? Country { get; set; }
        public string? Postcode { get; set; }
        public string? County { get; set; }
        public string? Town { get; set; }
        [JsonProperty("Website Address")] public string? WebsiteAddress { get; set; }
        [JsonProperty("Phone Number")] public string? PhoneNumber { get; set; }
        [JsonProperty("Address Line 4")] public string? Line4 { get; set; }
        [JsonProperty("Address LIne 3")] public string? Line3 { get; set; }
        [JsonProperty("Address Line 2")] public string? Line2 { get; set; }
        [JsonProperty("Address Line 1")] public string? Line1 { get; set; }
        [JsonProperty("Address Type")] public string? AddressType { get; set; }
    }
}