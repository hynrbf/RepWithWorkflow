namespace Common.Entities
{
    public class ProvidersProductDetails
    {
        public string? ProviderId { get; set; }
        public string? GroupName { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public bool IsAdvising { get; set; }
        public bool IsIntroducing { get; set; }
        public bool IsFinancialPromotions { get; set; }
        public string? Link { get; set; }
        public bool IsFinancialPromotionInfoOnly { get; set; }
        public string? RenumerationBasis { get; set; }
    }
}
