namespace Common.Entities
{
    public class FinancialStatus
    {
        public string? AnnualIncome { get; set; }
        public string? TotalAssets { get; set; }
        public string? TotalLiabilities { get; set; }
        public Money TotalAmountToActAsGuarantor { get; set; } = new Money();
        public string? SourceOfIncome { get; set; }
        public string? AdditionalInformation { get; set; }
    }
}
