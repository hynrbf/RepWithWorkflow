using Newtonsoft.Json;

namespace Common.Entities
{
    public class FinancialPromotion
    {
        [JsonProperty("id")] public string? Id { get; set; }
        public string? CustomerId { get; set; }
        public string? MediaOutlet { get; set; }
        public string ContentUrl { get; set; } = "";
        public bool IsRootUrl { get; set; }
        public StructuredContent? Content { get; set; }
        public EditorContent? EditorContent { get; set; }
        public string? Owner { get; set; }
        public string? Moderator { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
        public long ApprovalDays { get; set; }
        public bool IsLive { get; set; }
        public List<FileModel>? Media { get; set; }
        public string? Type { get; set; }
        public bool IsDisclosureConfirmed { get; set; }
        public List<string> ProductType { get; set; } = new();
        public string? RemunerationMethod { get; set; }
        public string? Provider { get; set; }
        public long CreatedAt { get; set; } = DateHelper.GetCurrentDateTimeInEpoch();
        public long? UpdatedAt { get; set; }

        public DateTime? LastScrapeDate { get; set; }
    }
}