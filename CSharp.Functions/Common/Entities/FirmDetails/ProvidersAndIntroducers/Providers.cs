namespace Common.Entities
{
    public class Providers : ProvidersAndIntroducersBase
    {
        public string? CustomerId { get; set; }
        public ProviderIntroducerDetails? Details { get; set; }
        public ProviderRepresentative? Representative { get; set; }
        public ProviderIntroducerDetails? PrincipalDetails { get; set; }
        public ProviderRepresentative? DepartmentDetails { get; set; }
        public MarketingRep? MarketingDetails { get; set; }
        public ProviderRepresentative? MarketingRepDetails { get; set; }
        public List<ProvidersProductDetails>? Products { get; set; }
        public List<ProvidersTaskDetails>? Tasks { get; set; }
        public string? FinancialNotes { get; set; }
        public string OnboardingType { get; set; } = OnboardingTypes.Provider.ToString();
    }
}
