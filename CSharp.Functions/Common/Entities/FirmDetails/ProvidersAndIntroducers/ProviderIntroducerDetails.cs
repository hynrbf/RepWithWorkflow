using Common.Entities.FirmDetails;

namespace Common.Entities
{
    public class ProviderIntroducerDetails
    {
        public string? Name { get; set; }
        public string? ForeName { get; set; }
        public string? LastName {  get; set; }
        public string? CompanyNumber { get; set; }
        public string? FcaFirmRefNo { get; set; }
        public string? PraFirmRefNo { get; set; }
        public string? RegisteredAddress { get; set; }
        public string? TradingAddress { get; set; }
        public string? HomeAddress { get; set; }
        public string? EmailAddress { get; set; }
        public ContactNumber? ContactNumber { get; set; }
        public string? Website { get; set; }
        public string? TradingName { get; set; }
        public string? Notes { get; set; }
        public bool? PraAuthorised { get; set; }
        public bool IsTradingSameAsRegisteredAddress { get; set; }
        public string? ContactNumberDisplay { get; set; }
        public bool IsCompany { get; set; }
        public List<ProductType> ProductType { get; set; } = new();
    }
}
