namespace Common.Entities
{
    public class InsuranceProvider
    {
        public string? ProviderName { get; set; }
        public int CompanyNumber { get; set; }
        public int FirmReferenceNumber { get; set; }
        public bool IsPraAuthorized { get; set; }
        public string? InsurerRating { get; set; }
        public string? RegisteredAddress { get; set; }
        public string? TradingAddress { get; set; }
        public string? EmailAddress { get; set; }
        public ContactNumber? ContactNumber { get; set; }
        public string? Website { get; set; }
        public bool WithBindingAuthority { get; set; }
        public bool HaveRiskTransferAgreement { get; set; }
        public bool AllowToCascadeRiskTransferAgreement { get; set; }
    }
}
