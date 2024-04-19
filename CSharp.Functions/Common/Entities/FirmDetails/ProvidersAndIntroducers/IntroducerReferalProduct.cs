namespace Common.Entities
{
    public class IntroducerReferalProduct
    {
        public bool IsReferralsprovided { get; set; }
        public string? FixedFeePerReferral { get; set; }
        public string? FixedFeeAndAPercentageOfRevenue { get; set; }
        public string? RemunerationVaryByProduct { get; set; }
        public string? RemuneratedInitiallyOnRenewal { get; set; }
        public int MonthsNotice { get; set; }
        public List<ScopeofReferalProduct>? Products { get; set; }
    }
}
