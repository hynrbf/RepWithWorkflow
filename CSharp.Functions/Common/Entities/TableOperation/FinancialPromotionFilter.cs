namespace Common.Entities
{
    public class FinancialPromotionFilter : Filter
    {
        public List<string> MediaOutlets { get; set; } = new();
        public List<string> Types { get; set; } = new();
        public List<string> ApprovalTypes { get; set; } = new();
        public string? ContentStatus { get; set; }
    }
}