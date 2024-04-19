namespace Common.Entities
{
    public class LocumCompanyDetails
    {
        public string? FirmName { get; set; }
        public string? CompanyNumber { get; set; }
        public string? FirmReferenceNumber { get; set; }
        public string? RegisteredAddress { get; set; }
        public bool IsRegisteredAddressChanged { get; set; }
        public string? TradingAddress { get; set; }
        public bool IsTradingAddressChanged { get; set; }
        public bool IsTradingSameAsRegisteredAddress { get; set; }
        public string? EmailAddress { get; set; }
        public ContactNumber? ContactNumber { get; set; }
        public string? Website { get; set; }
        public string? RepresentativeTitle { get; set; }
        public string? RepresentativeForename { get; set; }
        public string? RepresentativeSurname { get; set; }
        public string? RepresentativeEmailAddress { get; set; }
        public ContactNumber? RepresentativeMobileNumber { get; set; }
        public string? RepresentativeRole { get; set; }
    }
}
