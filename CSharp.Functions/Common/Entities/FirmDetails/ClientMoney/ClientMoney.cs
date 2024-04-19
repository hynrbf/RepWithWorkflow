namespace Common.Entities
{
    public class ClientMoney
    {
        public string? Id { get; set; }
        public ClassificationEnum? Classification { get; set; }
        public string? InsurerRating { get; set; }
        public string? ProviderName { get; set; }
        public int CompanyNumber { get; set; }
        public int FirmReferenceNumber { get; set; }
        public bool IsPraAuthorized { get; set; }
        public string? RegisteredAddress { get; set; }
        public string? TradingAddress { get; set; }
        public string? EmailAddress { get; set; }
        public ContactNumber? ContactNumber { get; set; }
        public string? Website { get; set; }
        public bool? WithBindingAuthority { get; set; }
        public long? DateAgreed { get; set; }
        public List<InsuranceProvider> InsuranceProviders { get; set; } = new();
        public bool? HaveRiskTransferAgreement { get; set; }
        public bool? AllowComminglingOfRiskTransferFunds { get; set; }
        public string? AccountType { get; set; }
        public List<FileModel>? RiskTransferAgreement { get; set; } = new();
        public List<FileModel>? StatutoryTrustLetter { get; set; } = new();
        public FirmRepresentativeDetails? FirmRepresentative { get; set; }
        public long? CreatedAt { get; set; }
        public long? UpdatedAt { get; set; }
    }
}