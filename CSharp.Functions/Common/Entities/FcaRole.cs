using Newtonsoft.Json;

namespace Common.Entities
{
    public class FcaRole
    {
        [JsonProperty("id")] public string? Id { get; set; }
        public string? Name { get; set; }
        public string? DisplayText { get; set; }
        public int Order { get; set; }
    }
}
