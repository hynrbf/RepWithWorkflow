namespace Common.Entities
{
    public class LocumSoleTraderDetails
    {
        public string? ForeName { get; set; }
        public string? SurName { get; set; }
        public string? FirmReferenceNumber { get; set; }
        public string? TradingAddress { get; set; }
        public bool IsTradingAddressChanged { get; set; }
        public string? EmailAddress { get; set; }
        public ContactNumber? ContactNumber { get; set; }
        public string? Website { get; set; }
    }
}
