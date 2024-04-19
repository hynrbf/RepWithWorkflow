using Newtonsoft.Json;

namespace Common.Entities
{
    public class AttributeHeaderK
    {
        [JsonProperty("string_head")] public string StringHead { get; set; }
        [JsonProperty("int_head")] public int IntHead { get; set; }
        [JsonProperty("bool_head")] public bool BoolHead { get; set; }
        [JsonProperty("float_head")] public double FloatHead { get; set; }
    }
}
