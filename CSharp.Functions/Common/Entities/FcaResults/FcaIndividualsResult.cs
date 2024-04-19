using Newtonsoft.Json;

namespace Common.Entities
{
    public class FcaIndividualsResult : FcaCompanyResultBase
    {
        public List<FcaIndividual> Data { get; set; } = new();
    }

    public class FcaIndividual
    {
        public string? Name { get; set; }
        public string? Status { get; set; }
        [JsonProperty("URL")] public string? Url { get; set; }
        [JsonProperty("IRN")] public string? Irn { get; set; }

        public List<FcaIndividualRole> CurrentRoles { get; set; } = new();
        public List<FcaIndividualRole> PreviousRoles { get; set; } = new();
    }
}