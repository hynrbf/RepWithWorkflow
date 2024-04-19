using Newtonsoft.Json;

namespace Common.Entities
{
    public class FcaCompanyK
    {
        [JsonProperty("URL")] public string? Url { get; set; }
        public string? Status { get; set; }
        [JsonProperty("Reference Number")] public string? ReferenceNo { get; set; }

        [JsonProperty("Type of business or Individual")]
        public string? TypeOfBusiness { get; set; }

        public string? Name { get; set; }
        public string? CompanyAddress { get; set; }
    }
}