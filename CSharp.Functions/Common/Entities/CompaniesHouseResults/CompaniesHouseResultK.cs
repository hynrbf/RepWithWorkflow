using Newtonsoft.Json;

namespace Common.Entities
{
    public class CompaniesHouseResultK
    {
        [JsonProperty("kind")] public string? Kind { get; set; }
        [JsonProperty("items")] public List<CompaniesHouseCompanyK> Items { get; set; }
        [JsonProperty("items_per_page")] public int ItemsPerPage { get; set; }
        [JsonProperty("total_results")] public int TotalResults { get; set; }
        [JsonProperty("page_number")] public int PageNumber { get; set; }
        [JsonProperty("start_index")] public int StartIndex { get; set; }
    }

    public class CompaniesHouseCompanyK
    {
        [JsonProperty("description")] public string? Description { get; set; }
        [JsonProperty("snippet")] public string? Snippet { get; set; }
        [JsonProperty("description_identifier")] public List<string> DescriptionIdentifier { get; set; }
        [JsonProperty("links")] public Links LinksK { get; set; }
        [JsonProperty("date_of_creation")] public string? DateOfCreation { get; set; }
        [JsonProperty("address_snippet")] public string? AddressSnippet { get; set; }
        [JsonProperty("kind")] public string? Kind { get; set; }
        [JsonProperty("title")] public string? Title { get; set; }
        [JsonProperty("company_status")] public string? CompanyStatus { get; set; }
        [JsonProperty("company_number")] public string? CompanyNumber { get; set; }
        [JsonProperty("address")] public Address AddressK { get; set; }
        [JsonProperty("company_type")] public string? CompanyType { get; set; }
        [JsonProperty("date_of_cessation")] public string? DateOfCessation { get; set; }
        [JsonProperty("external_registration_number")] public string? ExternalRegistrationNumber { get; set; }
    }

    public class Links
    {
        [JsonProperty("self")] public string? Self { get; set; }
    }

    public class ControllerLinks : Links
    {
        [JsonProperty("statement")] public string? Statement { get; set; }
    }

    public class Address
    {
        [JsonProperty("address_line_1")] public string? AddressLine1 { get; set; }
        [JsonProperty("address_line_2")] public string? AddressLine2 { get; set; }
        [JsonProperty("address_line_3")] public string? AddressLine3 { get; set; }
        [JsonProperty("address_line_4")] public string? AddressLine4 { get; set; }
        [JsonProperty("locality")] public string? Locality { get; set; }
        [JsonProperty("premises")] public string? Premises { get; set; }
        [JsonProperty("postal_code")] public string? PostalCode { get; set; }
        [JsonProperty("care_of_name")] public string? CareOfName { get; set; }
        [JsonProperty("care_of")] public string? CareOf { get; set; }
        [JsonProperty("country")] public string? Country { get; set; }
        [JsonProperty("region")] public string? Region { get; set; }
        [JsonProperty("po_box")] public string? PoBox { get; set; }
    }
}
