using Newtonsoft.Json;

namespace Common.Entities
{
    public class Setting
    {
        [JsonProperty("id")] public string Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string UsedFor { get; set; }
    }
}
