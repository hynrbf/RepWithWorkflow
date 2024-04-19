using Newtonsoft.Json;

namespace Common.Entities
{
    public class CurrencyConversion
    {
        [JsonProperty("id")] public string Id { get; set; }
        public bool Success { get; set; }
        public long Timestamp { get; set; }
        public string DateTimestamp { get; set; }
        public string Base { get; set; }
        public string Date { get; set; }
        public Dictionary<string, double> Rates { get; set; }
    }
}