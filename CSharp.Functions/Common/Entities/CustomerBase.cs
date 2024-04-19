namespace Common.Entities
{
    public abstract class CustomerBase : OnboardingUserBase
    {
        public Company? SelectedCompany { get; set; }
        public FirmRepresentativeDetails? FirmRepresentativeDetail { get; set; }
        public LocumDetails? LocumDetail { get; set; }
        public AccountDepartmentDetail? AccountDepartmentDetail { get; set; }
        public List<IndividualController?> IndividualControllers { get; set; } = new();
        public List<CorporateController?> CorporateControllers { get; set; } = new();
        public int NoOfIndividualShareholders => IndividualControllers.Count;
        public int NoOfCorporateShareholders => CorporateControllers.Count;
        public List<CloseLink?> CloseLinks { get; set; } = new List<CloseLink?>();
        public List<ProfessionalIndemnity> ProfessionalIndemnities { get; set; } = new();
        public List<EmployersLiability> EmployersLiabilities { get; set; } = new();
        public List<Stationery>? Stationeries { get; set; }
        public Disclosure? Disclosure { get; set; }
        public DataProtectionLicense? DataProtectionLicense { get; set; }
        public List<MediaMarketingOutlet> MediaMarketingOutlets { get; set; } = new();
        public List<Affiliate> Affiliates { get; set; } = new();
        public bool OnboardingCompleted { get; set; }
        public string ProfileStatus { get; set; } = ProfileStatuses.Basic.ToString();

        public bool IsInProgressDataInitializing { get; set; }
    }
}
