namespace Common.Entities
{
    public class ScopeofReferalProduct
    {
        public string? IntroducerId { get; set; }
        public string? GroupName { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? ReferralStatus { get; set; }
        public string? Limitations { get; set; }
        public decimal? InitialRenumeration { get; set; }
        public double? InitialRenumerationPercentage { get; set; }
        public decimal? RenewalTrailRenuneration { get; set; }
        public double? RenewalTrailRenumerationPercentage { get; set; }
    }
}
