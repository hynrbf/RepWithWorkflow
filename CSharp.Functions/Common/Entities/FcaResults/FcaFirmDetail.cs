using Newtonsoft.Json;

namespace Common.Entities
{
    public class FcaFirmDetail
    {

        [JsonProperty("Organisation Name")]
        public string? OrganisationName { get; set; }

        [JsonProperty("FRN")]
        public string? FirmReferenceNumber { get; set; }


        [JsonProperty("Status Effective Date")]
        public string? StatusEffectiveDate { get; set; }


        [JsonProperty("Companies House Number")]
        public string? CompaniesHouseNumber { get; set; }


        [JsonProperty("Business Type")]
        public string? BusinessType { get; set; }

        [JsonProperty("Client Money Permission")]
        public string? ClientMoneyPermission { get; set; }

        public string? Status { get; set; }
    }
}
