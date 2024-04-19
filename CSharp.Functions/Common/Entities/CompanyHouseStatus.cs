using Newtonsoft.Json;

namespace Common.Entities
{
    public class CompanyHouseStatus
    {
        [JsonProperty("id")] 
        public string Id { get; set; }
        public string Status { get; set; }
        public string ColorCoding { get; set; }
        public bool IsAuthorised { get; set; }
        public string StatusDisplayText { get; set; }
    }
}
