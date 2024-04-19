namespace Common.Entities
{
    public abstract class ProvidersAndIntroducersBase : CustomerBase
    {
        public long? StartDate { get; set; }
        public string? Status { get; set; }
        public string? DdStatus { get; set; }
        public string? StatusImg { get; set; }
        public string? ProviderImg { get; set; }
    }
}
