namespace Common.Entities
{
    public class ProfessionalIndemnity : InsurerBrokerInformation
    {
        public string? InsurerName { get; set; }
        public string? CompanyNumber { get; set; }
        public string? FirmReferenceNumber { get; set; }
        public string? PiiBrokerName { get; set; }
        public string? BrokerCompanyNumber { get; set; }
        public string? BrokerFirmReferenceNumber { get; set; }
        public string? PolicyNumber { get; set; }
        public Money? PremiumAmount { get; set; }
        public Money? AnnualIncome { get; set; }
        public long? RetroactiveStartDate { get; set; }
        public long? StartDate { get; set; }
        public long? EndDate { get; set; }
        public List<string> BusinessLinesCovered { get; set; } = new();
        public List<LimitIndemnity> LimitIndemnitiesSingle { get; set; } = new();
        public List<LimitIndemnity> LimitIndemnitiesAggregate { get; set; } = new();
        public List<PolicyExcess> PolicyExcesses { get; set; } = new();
        public List<PolicyExclusion> PolicyExclusions { get; set; } = new();
        public List<string> AppointedRepresentatives { get; set; } = new();
        public List<string> BusinessLineSubjectToLimitOfIndemnityItemsSingle { get; set; } = new();
        public List<string> BusinessLineSubjectToLimitOfIndemnityItemsAggregate { get; set; } = new();
        public List<string> BusinessLineCategoriesSubjectToPolicyExclusions { get; set; } = new();
        public List<string> BusinessLineCategoriesSubjectToPolicyExcess { get; set; } = new();
        public List<FileModel> ProposalFormFile { get; set; } = new();
        public List<FileModel> SchedOfInsurance { get; set; } = new();
    }
}