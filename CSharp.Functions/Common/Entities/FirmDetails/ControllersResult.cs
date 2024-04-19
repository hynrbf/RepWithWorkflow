using Newtonsoft.Json;

namespace Common.Entities
{
    public class ControllersResult
    {
        public Links? Links { get; set; }
        [JsonProperty("start_index")] public int StartIndex { get; set; }
        [JsonProperty("items")] public List<Controller> Controllers { get; set; } = new();
        [JsonProperty("total_results")] public int TotalResults { get; set; }
        [JsonProperty("ceased_count")] public int CeasedCount { get; set; }
        [JsonProperty("active_count")] public int ActiveCount { get; set; }
    }
}