namespace Common.Entities
{
    public abstract class CompanyDetails
    {
        public string? CompanyName { get; set; }
        public string? CountryOfIncorporation { get; set; }
        public string? CountryCode { get; set; }
        public string? CompanyNumber { get; set; }
        public string? FirmReferenceNumber { get; set; }
        public string? NatureOfBusiness { get; set; }
    }
}
