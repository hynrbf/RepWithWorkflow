using Newtonsoft.Json;

namespace Common.Entities
{
    public abstract class FcaCompanyResultBase
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public ResultInfo? ResultInfo { get; set; }
    }

    public class ResultInfo
    {
        public string? Next { get; set; }
        public string? Page { get; set; }
        [JsonProperty("per_page")] public string? PerPage { get; set; }
        [JsonProperty("total_count")] public string? TotalCount { get; set; }
    }
}