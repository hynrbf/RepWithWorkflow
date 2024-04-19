using Newtonsoft.Json;

namespace Common.Entities
{
    public class EventAttributesK
    {
        [JsonProperty("callback")] public string CallBack { get; set; }
        [JsonProperty("use_tls_12")] public bool isUseTls12 { get; set; } = true;
        [JsonProperty("docid_queryparam")] public bool isDocIdQueryParam { get; set; } = true;
        [JsonProperty("headers")] public AttributeHeaderK Headers { get; set; }
    }
}
