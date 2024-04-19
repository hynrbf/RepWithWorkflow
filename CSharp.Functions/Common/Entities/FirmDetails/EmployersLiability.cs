namespace Common.Entities
{
    public class EmployersLiability : InsurerBrokerInformation
    {
        public FirmBasicInfo Insurer { get; set; } = new();
        public FirmBasicInfo Broker { get; set; } = new();
        public Money PremiumAmount { get; set; } = new();
        public Money SingleIndemnityLimit { get; set; } = new();
        public Money AggregateIndemnityLimit { get; set; } = new();
        public Money PolicyExcess { get; set; } = new();
        public List<FileModel> ProposalFormFile { get; set; } = new();
        public List<FileModel> SchedOfInsurance { get; set; } = new();
        public string? PaymentFrequency { get; set; }
        public string? PolicyExclusions { get; set; }
        public long? RetroactiveStartDate { get; set; }
        public long? StartDate { get; set; }
        public long? EndDate { get; set; }
    }
}