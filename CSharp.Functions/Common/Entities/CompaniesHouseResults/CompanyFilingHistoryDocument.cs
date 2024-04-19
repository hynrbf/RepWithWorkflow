using Newtonsoft.Json;

namespace Common.Entities;

public class CompanyFilingHistoryDocument
{
    public string? Filename { get; set; }
    [JsonProperty("pages")] public int PageCount { get; set; }
    [JsonProperty("created_at")] public DateTime CreatedDate { get; set; }
    [JsonProperty("updated_at")] public DateTime UpdatedDate { get; set; }
}