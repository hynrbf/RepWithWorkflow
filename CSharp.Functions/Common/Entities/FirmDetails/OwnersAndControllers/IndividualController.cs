namespace Common.Entities
{
    public class IndividualController
    {
        public IndividualControllerDetails? Detail { get; set; }
        public List<CompanyOfficerAppointmentDetails> DirectorsAndDirectorship { get; set; } = new();
        public List<CompanyControllingInterestDetails> ControllingInterests { get; set; } = new();
        public FinancialStatus? FinancialStatus { get; set; }
        public List<string> CurriculumVitaeUrls { get; set; } = new();
    }
}