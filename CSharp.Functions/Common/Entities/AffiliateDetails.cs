namespace Common.Entities
{
    public class AffiliateDetails
    {
        public string? Name { get; set; }
        public string? CompanyNumber { get; set; }
        public string? FirmReferenceNumber { get; set; }
        public bool isPRAAuthorised { get; set; } = false;
        public string? RegisteredAddress { get; set; }
        public string? EmailAddress { get; set; }
        public ContactNumber? ContactNumber { get; set; }
        public string? Website { get; set; }
        public string? TradingAddress { get; set; }
        public bool IsTradingAddressChanged { get; set; } = false;
    }
}
