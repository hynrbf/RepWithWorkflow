using Newtonsoft.Json;
using System.Globalization;

namespace Common.Entities
{
    //we use this as both contract and entity
    public class Controller : CompanyDetails
    {
        public long DateOfBirth
        {
            get
            {
                if (BirthDate == null)
                {
                    return 0;
                }

                var day = BirthDate.Day == 0 ? 1 : BirthDate.Day;
                var month = BirthDate.Month == 0 ? 1 : BirthDate.Month;
                var year = BirthDate.Year == 0 ? 1 : BirthDate.Year;

                var dateTime = new DateTime(year, month, day);
                return DateHelper.ConvertDateTimeToEpoch(dateTime);
            }
            set
            {
                var date = DateHelper.ConvertEpochToDateTime(value);

                BirthDate = new DateData
                {
                    Year = date.Year,
                    Month = date.Month,
                    Day = date.Day
                };
            }
        }

        public long? DateCeased
        {
            get
            {
                if (string.IsNullOrEmpty(CeasedOn))
                {
                    return null;
                }

                var dateTimeObj = DateTime.ParseExact(CeasedOn, AppConstants.FcaDateFormatFromApiResult,
                    CultureInfo.InvariantCulture);
                return DateHelper.ConvertDateTimeToEpoch(dateTimeObj);
            }
            set => CeasedOn = value == null
                ? null
                : DateHelper.ConvertEpochToDateTime((long)value).ToString(AppConstants.FcaDateFormatFromApiResult,
                    CultureInfo.InvariantCulture);
        }

        public long DateNotified
        {
            get
            {
                if (string.IsNullOrEmpty(CeasedOn))
                {
                    return 0;
                }

                var dateTimeObj = DateTime.ParseExact(CeasedOn, AppConstants.FcaDateFormatFromApiResult,
                    CultureInfo.InvariantCulture);
                return DateHelper.ConvertDateTimeToEpoch(dateTimeObj);
            }
            set
            {
                CeasedOn = DateHelper.ConvertEpochToDateTime(value)
                    .ToString(AppConstants.FcaDateFormatFromApiResult, CultureInfo.InvariantCulture);
            }
        }

        public double? PercentageOfCapital => GetPercent("ownership-of-shares");
        public double? PercentageOfVotingRights => GetPercent("voting-rights");

        public List<Controller> IndividualControllers { get; set; } = new();
        public List<Controller> CorporateControllers { get; set; } = new();
        public CompanyOfficerAppointmentDetails? Directorships { get; set; }
        public List<CompanyControllingInterestDetails> ControllingInterests { get; set; } = new();
        public List<CompanyOfficer>? Directors { get; set; }
        public Address? Address { get; set; }
        public string? FullAddress { get; set; }
        public string? Description { get; set; }
        public Identification? Identification { get; set; }
        public string? Kind { get; set; }
        public ControllerLinks? Links { get; set; }
        public string? Name { get; set; }
        public string? Nationality { get; set; }
        [JsonProperty("date_of_birth")] public DateData? BirthDate { get; set; }
        [JsonProperty("ceased")] public bool IsCeased { get; set; }
        [JsonProperty("ceased_on")] public string? CeasedOn { get; set; }
        [JsonProperty("country_of_residence")] public string? CountryOfResidence { get; set; }
        [JsonProperty("etag")] public string? ETag { get; set; }
        [JsonProperty("is_sanctioned")] public bool IsSanctioned { get; set; }
        [JsonProperty("name_elements")] public NameElement? NameElements { get; set; }
        [JsonProperty("natures_of_control")] public List<string> NaturesOfControl { get; set; } = new List<string>();
        [JsonProperty("notified_on")] public string? NotifiedOn { get; set; }

        [JsonProperty("principal_office_address")]
        public Address? PrincipalOfficeAddress { get; set; }

        private double? GetPercent(string controlType)
        {
            if (!NaturesOfControl.Any())
            {
                return null;
            }

            if (NaturesOfControl.Any(n => n.Contains($"{controlType}-25-to-50-percent")))
            {
                return 25.0;
            }

            if (NaturesOfControl.Any(n => n.Contains($"{controlType}-50-to-75-percent")))
            {
                return 50.0;
            }

            return NaturesOfControl.Any(n => n.Contains($"{controlType}-75-to-100-percent")) ? 75.0 : null;
        }
    }
}