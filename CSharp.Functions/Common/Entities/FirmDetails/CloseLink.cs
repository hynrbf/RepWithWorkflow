namespace Common.Entities
{
    public class CloseLink : CompanyDetails
    {
        public string? NatureOfCloseLink { get; set; }
        public string? RegisteredAddress { get; set; }
        public string? TradingAddress { get; set; }
        public bool IsTradingAddressChanged { get; set; }
        public bool IsTradingSameAsRegisteredAddress { get; set; }
        public string? EmailAddress { get; set; }
        public string? Website { get; set; }
        public ContactNumber ContactNumber { get; set; } = new ContactNumber();
        public FirmBasicInfo Firm { get; set; } = new FirmBasicInfo();
        public double? PercentageOfCapital { get; set; }
        public double? PercentageOfVotingRights { get; set; }
    }
}
