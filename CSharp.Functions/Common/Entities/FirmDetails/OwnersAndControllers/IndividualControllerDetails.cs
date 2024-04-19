namespace Common.Entities
{
    public class IndividualControllerDetails
    {
        public string? Title { get; set; }
        public string? Forename { get; set; }
        public string? Surname { get; set; }
        public string? PreviousFullName { get; set; }
        public string? ReasonForChangeName { get; set; }
        public long DateOfNameChange { get; set; }
        public string? CommonlyUsedName { get; set; }
        public ContactNumber? ContactNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? HomeAddress { get; set; }
        public bool IsHomeAddressChanged { get; set; }
        public long? HomeAddressResidenceDate { get; set; }
        public List<AddressInfo> PreviousAddresses { get; set; } = new();
        public long DateOfBirth { get; set; }
        public string? CountryOfBirth { get; set; }
        public List<string> Nationalities { get; set; } = new();
        public List<string> PreviousNationalities { get; set; } = new();
        public string? NationalInsuranceNumber { get; set; }
        public string? PassportNumber { get; set; }
        public double? PercentageOfCapital { get; set; }
        public double? PercentageOfVotingRights { get; set; }
        public bool HasBeenSubjectToAnyMaterialComplaints { get; set; }
        public string? AdditionalInformation { get; set; }
        public List<string> SupportingDocumentsUrls { get; set; } = new();
    }
}