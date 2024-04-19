namespace Common.Entities
{
    public class Company
    {
        public string CompanyName { get; set; }
        public string Postcode { get; set; }
        public string Address { get; set; }
        public string CompanyNumber { get; set; }
        public string FirmReferenceNo { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public bool IsAuthorized { get; set; }
        public bool IsConfirmedFirmDetails { get; set; }
        public bool IsVariedFirmPermissions { get; set; }
        public bool IsSelected { get; set; }
        public string Region { get; set; }
        public string CompanyStatus { get; set; }
        public List<FcaAppointedRepresentative> AppointedRepresentatives { get; set; } = new List<FcaAppointedRepresentative>();
    }
}
