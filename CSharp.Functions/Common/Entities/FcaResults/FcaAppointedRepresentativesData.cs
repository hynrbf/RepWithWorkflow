namespace Common.Entities
{
    public class FcaAppointedRepresentativesData
    {
        public List<FcaAppointedRepresentative> PreviousAppointedRepresentatives { get; set; } =
            new List<FcaAppointedRepresentative>();

        public List<FcaAppointedRepresentative> CurrentAppointedRepresentatives { get; set; } =
            new List<FcaAppointedRepresentative>();
    }
}