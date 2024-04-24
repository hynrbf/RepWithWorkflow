namespace Common.Entities
{
    public class Affiliate : ProvidersAndIntroducersBase
    {
        public string? Id { get; set; }
        public AffiliateDetails? Details { get; set; }
        public AffiliateDetails? MarketingProviderDetails { get; set; }
        public bool IsAffiliateMarketingProvider { get; set; } = false;
        public ProviderRepresentative? Representative { get; set; }
        public string OnboardingType { get; set; } = OnboardingTypes.Affiliate.ToString();
    }
}
