using Newtonsoft.Json;

namespace Common.Entities;

public class CompanyFilingHistoryResult
{
    public string? ETag { get; set; }
    [JsonProperty("filing_history_status")] public string? FilingHistoryStatus { get; set; }
    public IEnumerable<CompanyFilingHistoryItem> Items { get; set; } = new List<CompanyFilingHistoryItem>();
    [JsonProperty("items_per_page")] public int ItemsPerPage { get; set; }
    [JsonProperty("kind")] public string? Kind { get; set; }
    [JsonProperty("start_index")] public int StartIndex { get; set; }
    [JsonProperty("total_count")] public int TotalCount { get; set; }
}

public class CompanyFilingHistoryItem
{
    [JsonProperty("annotations")] public IEnumerable<Annotation> Annotations { get; set; } = new List<Annotation>();

    [JsonProperty("associated_filings")]
    public IEnumerable<AssociatedFiling> AssociatedFilings { get; set; } = new List<AssociatedFiling>();

    public DateTime? Date { get; set; }

    [JsonProperty("transaction_id")] public string TransactionId { get; set; }

    public string Type { get; set; }

    public string Description { get; set; }

    public DocumentLink Links { get; set; }
}

public class DocumentLink
{
    public string Self { get; set; }

    [JsonProperty("document_metadata")] public string DocumentMetadata { get; set; }

    public string DocumentId
    {
        get
        {
            if (string.IsNullOrEmpty(DocumentMetadata)) return "";

            var uri = new Uri(DocumentMetadata);
            var segments = uri.Segments;

            return segments[^1].Trim('/');
        }
    }
}

public class Annotation
{
    [JsonProperty("annotation")] public string? AnnotationK { get; set; }
    public DateTime? Date { get; set; }
    public string? Description { get; set; }
}

public class AssociatedFiling
{
    private DateTime? Date { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
}