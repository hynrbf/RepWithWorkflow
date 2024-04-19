namespace Common.Entities
{
    public class InsurerBrokerInformation
    {
        public string? InsurerRegisteredAddress { get; set; }
        public string? InsurerTradingAddress { get; set; }
        public bool InsurerIsTradingSameAsRegisteredAddress { get; set; }
        public string? InsurerEmailAddress { get; set; }
        public ContactNumber? InsurerContactNumber { get; set; }
        public string? InsurerWebsite { get; set; }
        public bool? IsBrokerResponsible { get; set; }
        public bool IsBrokerCompany { get; set; } = true;
        public string? BrokerRegisteredAddress { get; set; }
        public string? BrokerTradingAddress { get; set; }
        public bool BrokerIsTradingSameAsRegisteredAddress { get; set; }
        public string? BrokerEmailAddress { get; set; }
        public ContactNumber? BrokerContactNumber { get; set; }
        public string? BrokerWebsite { get; set; }
    }
}
