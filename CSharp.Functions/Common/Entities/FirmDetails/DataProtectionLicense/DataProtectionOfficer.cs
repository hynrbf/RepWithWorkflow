namespace Common.Entities
{
    public class DataProtectionOfficer : Individual
    {
        public bool IsOrganisation { get; set; }

        public string? CompanyName { get; set; }
        public string? CompanyNumber { get; set; }
        public string? FirmReferenceNo { get; set; }
    }
}