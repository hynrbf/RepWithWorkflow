namespace Common.Entities
{
    public class CorporateController : CompanyDetails
    {
        public string? FirmType { get; set; }
        public string? FinancialSolvency { get; set; }
        public string? RegisteredAddress { get; set; }
        public bool IsRegisteredAddressChanged { get; set; }
        public bool IsTradingAddressSameAsRegisteredAddress { get; set; }
        public string? TradingAddress { get; set; }
        public bool IsTradingAddressChanged { get; set; }
        public bool IsHeadOfficeAddressSameAsTradingAddress { get; set; }
        public string? HeadOfficeAddress { get; set; }
        public string? EmailAddress { get; set; }
        public ContactNumber? ContactNumber { get; set; }
        public double? PercentageOfCapital { get; set; }
        public double? PercentageOfVotingRights { get; set; }
        public double IndividualOwners { get; set; }
        public int CorporateOwners { get; set; }
        public bool IsSubjectToRegulationByAnotherRegulator { get; set; }
        public bool IsThirdCountryFirm { get; set; }
        public string? ThirdCountryFirmInfo { get; set; }
        public bool IsMemberOfFinancialConglomerate { get; set; }
        public string? MemberOfFinancialConglomerateInfo { get; set; }
        public bool IsMemberOfThirdCountryFinancialConglomerate { get; set; }
        public string? MemberOfThirdCountryFinancialConglomerateInfo { get; set; }
        public bool IsMemberOfThirdCountryBanking { get; set; }
        public string? MemberOfThirdCountryBankingInfo { get; set; }
        public bool HasBeenSubjectToAnyMaterialComplaints { get; set; }
        public string? BeenSubjectToAnyMaterialComplaintsInfo { get; set; }
        public List<Director?> Directors { get; set; } = new();
        public List<IndividualController> IndividualControllers { get; set; } = new();
        public List<CorporateController> CorporateControllers { get; set; } = new();
        public List<string> SupportingDocumentsUrls { get; set; } = new();
    }
}