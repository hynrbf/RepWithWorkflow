namespace Common.Entities
{
    public class Introducer : ProvidersAndIntroducersBase
    {
        public string? CustomerId { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? SortCode { get; set; }
        public string? AccountNumber { get; set; }
        public string? FcaAuthorisationStatus { get; set; }
        public ProviderIntroducerDetails? Details { get; set; }
        public ProviderRepresentative? Representative { get; set; }   
        public IntroducerDetails? PrincipalDetails { get; set; }
        public ProviderRepresentative? DepartmentDetails { get; set; }
        public IntroducerReferalProduct? Referral { get; set; }
        public string OnboardingType { get; set; } = OnboardingTypes.Introducer.ToString();
    }
}