using Newtonsoft.Json;

namespace Common.Entities
{
    public class CompanyProfileK
    {
        public string? ETag { get; set; }
        public string? Jurisdiction { get; set; }
        [JsonProperty("company_name")] public string? CompanyName { get; set; }
        [JsonProperty("company_number")] public string? CompanyNumber { get; set; }
        [JsonProperty("company_status")] public string? CompanyStatus { get; set; }
        [JsonProperty("date_of_creation")] public string? DateOfCreation { get; set; }
        [JsonProperty("date_of_cessation")] public string? DateOfCessation { get; set; }
        public string? Type { get; set; }
        [JsonProperty("registered_office_address")] public Address? RegisteredOfficeAddress { get; set; }
        public CompanyProfileLink? Links { get; set; }
        [JsonProperty("sic_codes")] public List<string> SicCodes { get; set; } = new();
        public CompanyProfileAccounts? Accounts { get; set; }
    }

    public class CompanyProfileLink : Links
    {
        [JsonProperty("filing_history")] public string? FilingHistory { get; set; }
        public string? Officers { get; set; }
        [JsonProperty("persons_with_significant_control")] public string? PersonWithSignificantControl { get; set; }
    }
}
