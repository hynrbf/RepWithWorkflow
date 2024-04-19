using Common.Infra;
using Newtonsoft.Json;

namespace Common.Entities
{
    public class CompanyOfficersResult
    {
        public IEnumerable<CompanyOfficer> Items { get; set; } = new List<CompanyOfficer>();
        public string? Kind { get; set; }
        public string? ETag { get; set; }
        [JsonProperty("items_per_page")] public int ItemsPerPage { get; set; }
        [JsonProperty("start_index")] public int StartIndex { get; set; }
        [JsonProperty("total_results")] public int TotalResults { get; set; }
        [JsonProperty("inactive_count")] public int InactiveCount { get; set; }
        [JsonProperty("resigned_count")] public int ResignedCount { get; set; }
        [JsonProperty("active_count")] public int ActiveCount { get; set; }
    }

    public class CompanyOfficer
    {
        public long DateOfBirth =>
            BirthDate == null
                ? 0
                : DateHelper.ConvertDateTimeToEpoch(new DateTime(BirthDate.Year, BirthDate.Month,
                    BirthDate.Day == 0 ? 1 : BirthDate.Day));

        public string Forename
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                {
                    return string.Empty;
                }

                var segments = Name.Split(',');

                return segments.Length < 2 ? string.Empty : Helpers.CapitalizeOnlyFirstLetterOfWord(segments[1].Trim());
            }
        }

        public string Surname
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                {
                    return string.Empty;
                }

                var segments = Name.Split(',');

                return segments.Length < 1 ? string.Empty : Helpers.CapitalizeOnlyFirstLetterOfWord(segments[0].Trim());
            }
        }

        public string? Position => Occupation;
        public string? Title { get; set; }
        public string? Name { get; set; }
        public string? Nationality { get; set; }
        public string? ETag { get; set; }
        public string? Occupation { get; set; }
        public string? Responsibilities { get; set; }
        [JsonProperty("officer_role")] public string? OfficerRole { get; set; }
        [JsonProperty("country_of_residence")] public string? CountryOfResidence { get; set; }
        [JsonProperty("appointed_before")] public string? AppointedBefore { get; set; }
        [JsonProperty("appointed_on")] public string? AppointedOn { get; set; }
        [JsonProperty("resigned_on")] public string? ResignedOn { get; set; }
        public Address? Address { get; set; }
        public OfficerLink? Links { get; set; }

        [JsonProperty("principal_office_address")]
        public Address? PrincipalOfficeAddress { get; set; }

        [JsonProperty("date_of_birth")] public DateOfBirth? BirthDate { get; set; }
        [JsonProperty("contact_details")] public ContactDetails? ContactDetails { get; set; }
        [JsonProperty("former_names")] public List<Name> FormerNames { get; set; } = new List<Name>();
        public Identification? Identification { get; set; }

        [JsonProperty("is_pre_1992_appointment")]
        public bool IsPre1992Appointment { get; set; }
    }

    public class ContactDetails
    {
        public string? Name { get; set; }
    }

    public class DateOfBirth
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class Name
    {
        public string? Forenames { get; set; }
        public string? Surname { get; set; }
    }

    public class Identification
    {
        [JsonProperty("identification_type")] public string? IdentificationType { get; set; }
        [JsonProperty("legal_authority")] public string? LegalAuthority { get; set; }
        [JsonProperty("legal_form")] public string? LegalForm { get; set; }
        [JsonProperty("place_registered")] public string? PlaceRegistered { get; set; }
        [JsonProperty("registration_number")] public string? RegistrationNumber { get; set; }
    }

    public class OfficerLink : Links
    {
        public OfficerLinkDetails? Officer { get; set; }
    }

    public class CompanyLink
    {
        public string? Company { get; set; }
    }

    public class OfficerLinkDetails
    {
        public string? Appointments { get; set; }
    }
}