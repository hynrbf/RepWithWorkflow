using Newtonsoft.Json;

namespace Common.Entities
{
    public class NameElement
    {
        public string? Title { get; set; }
        public string? Forename { get; set; }
        [JsonProperty("middle_name")] public string? MiddleName { get; set; }
        public string? Surname { get; set; }
        [JsonProperty("other_forenames")] public string? OtherForenames { get; set; }
    }
}