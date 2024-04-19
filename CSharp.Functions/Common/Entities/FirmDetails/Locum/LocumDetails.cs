namespace Common.Entities
{
    public class LocumDetails
    {
        public bool IsDependentOnSingleKeyIndividual { get; set; }
        public bool? IsLocumCompany { get; set; }
        public bool? HasAppointedALocum { get; set; }
        public string? HasNoAppointedLocumInfo { get; set; }
        public LocumCompanyDetails? LocumCompanyDetail { get; set; }
        public LocumSoleTraderDetails? LocumSoleTraderDetail { get; set; }
    }
}
