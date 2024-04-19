namespace Common.Entities
{
    public class AppointedRepresentative : CustomerBase
    {
        public string? CustomerId { get; set; }
        public bool? IsCompany { get; set; }
        public string? Name { get; set; }
        public List<string> TradingNames { get; set; } = new();
        public bool IsTradingNamesChanged { get; set; }
        public bool IsTradingSameAsRegisteredAddress { get; set; }
        public long ProposedCommencementDate { get; set; }
        public string? RegisteredAddress { get; set; }
        public string? TradingAddress { get; set; }
        public string? Website { get; set; }
        public string? Title { get; set; }
        public string? Forename { get; set; }
        public string? Surname { get; set; }
        public string? HomeAddress { get; set; }
        public long ResidenceStartDate { get; set; }
        public long? DateOfBirth { get; set; }
        public string? CountryOfBirth { get; set; }
        public string? Nationality { get; set; }
        public List<string> Nationalities { get; set; } = new();
        public string? NationalityInsuranceNumber { get; set; }
        public string? PassportNumber { get; set; }
        public FirmRepresentativeDetails Representative { get; set; } = new();
        public List<ArActivity> Activities { get; set; } = new();
        public bool? IsBeIntroducer { get; set; }
        public string? PrimaryReason { get; set; }
        public bool? IsPayForServices { get; set; }
        public string? ServiceToPayFor { get; set; }
        public List<ArFee> Fees { get; set; } = new();
        public bool? IsActivitiesCoveredByPii { get; set; }
        public string? PiiActivitiesCovered { get; set; }
        public ArStatus? Status { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }

        // TODO: Under review if needed
        public string? Url { get; set; }
        public string? Frn { get; set; }
        public bool IsCurrentRepresentative { get; set; }
        public string? PrincipalFirmName { get; set; }
        public string? PrincipalFrn { get; set; }
        public bool IsEeaTiedAgent { get; set; }
        public bool IsTiedAgent { get; set; }
        public long TerminationDate { get; set; }
        public string? TerminationDateStr { get; set; }
        public long EffectiveDate { get; set; }
        public string? EffectiveDateStr { get; set; }
        public int FcaFirmRefNo { get; set; }
        public string? FirmType { get; set; }
        public bool IsIntroducer { get; set; }
        public string? Service { get; set; }
        public ARAdditionalInformation? AdditionalInformation { get; set; }

        public string OnboardingType { get; set; } = OnboardingTypes.Ar.ToString();
        public BankAccountDetails? BankAccountDetails { get; set; }
        public ARFirmDetail? FirmDetail { get; set; }
        public List<Providers> Providers { get; set; } = new();
        public DataProtectionLicense? DataProtectionLicenseCache { get; set; }
        public List<Introducer> Introducers { get; set; } = new();
        public bool IsActivityAdvisorDriven { get; set; }
        public bool HasAdditionalServices { get; set; }
        public Money? ForecastedAnnualRevenueActivities { get; set; }
        public Money? ForecastedAnnualRevenueServices { get; set; }
        public long? PreferredCommencementDate { get; set; }
    }
}