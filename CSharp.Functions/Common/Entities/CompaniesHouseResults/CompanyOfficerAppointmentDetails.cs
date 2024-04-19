using Newtonsoft.Json;
using System.Globalization;

namespace Common.Entities
{
    //we use this as both contract and entity
    public class CompanyOfficerAppointmentDetails
    {
        [JsonProperty("date_of_birth")]
        public DateData DateOfBirthData { get; set; } = new DateData { Day = 1, Month = 1, Year = 1 };

        [JsonProperty("items")]
        public List<CompanyOfficeAppointment> Appointments { get; set; } = new List<CompanyOfficeAppointment>();

        public string? Name { get; private set; }

        public string FullName
        {
            get =>
                string.IsNullOrEmpty(Name) ? string.Empty : Helpers.CapitalizeOnlyFirstLetterOfWord(Name.Trim());
            set => Name = value;
        }

        public long DateOfBirth
        {
            get => DateHelper.ConvertDateTimeToEpoch(new DateTime(DateOfBirthData.Year, DateOfBirthData.Month,
                DateOfBirthData.Day));
            set
            {
                var date = DateHelper.ConvertEpochToDateTime(value);

                DateOfBirthData = new DateData
                {
                    Year = date.Year,
                    Month = date.Month,
                    Day = date.Day
                };
            }
        }

        public List<string> Nationalities { get; set; } = new();
    }

    public class CompanyOfficeAppointment
    {
        private const string PresentDateValue = "Present";
        private const long PresentDateLongValue = -1;

        public Address? Address { get; set; }
        [JsonProperty("appointed_to")] public CompanyOfAppointment? AppointedTo { get; set; }
        public string? Name { get; set; }
        [JsonProperty("officer_role")] public string? OfficerRole { get; set; }
        [JsonProperty("appointed_on")] public string? AppointedOn { get; set; }
        [JsonProperty("resigned_on")] public string? ResignedOn { get; set; }
        public string? NatureOfBusiness { get; set; }
        public string? FirmReferenceNumber { get; set; }
        [JsonProperty("name_elements")] public NameElement? NameElements { get; set; }

        public string CompanyStatus => AppointedTo?.CompanyStatus ?? "";

        public string? CompanyNumber
        {
            get => AppointedTo?.CompanyNumber;
            set
            {
                AppointedTo ??= new CompanyOfAppointment();
                AppointedTo.CompanyNumber = value;
            }
        }

        public string? CompanyName
        {
            get => AppointedTo?.CompanyName;
            set
            {
                AppointedTo ??= new CompanyOfAppointment();
                AppointedTo.CompanyName = value;
            }
        }

        public string? Country
        {
            get => Address?.Country;
            set
            {
                Address ??= new Address();
                Address.Country = value;
            }
        }

        public string? CountryCode { get; set; }

        public string? OriginalResignedOn { get; set; }

        public long? DirectorshipStartDate
        {
            get
            {
                if (string.IsNullOrEmpty(AppointedOn))
                {
                    return null;
                }

                var dateTimeObj = DateTime.ParseExact(AppointedOn, AppConstants.FcaDateFormatFromApiResult, CultureInfo.InvariantCulture);
                return DateHelper.ConvertDateTimeToEpoch(dateTimeObj);
            }
            set => AppointedOn = value == null
                ? null
                : DateHelper.ConvertEpochToDateTime((long)value).ToString(AppConstants.FcaDateFormatFromApiResult, CultureInfo.InvariantCulture);
        }

        public long? DirectorshipEndDate
        {
            get
            {
                if (string.IsNullOrEmpty(ResignedOn) || ResignedOn == PresentDateValue)
                {
                    return PresentDateLongValue;
                }

                var dateTimeObj = DateTime.ParseExact(ResignedOn, AppConstants.FcaDateFormatFromApiResult, CultureInfo.InvariantCulture);
                return DateHelper.ConvertDateTimeToEpoch(dateTimeObj);
            }
            set
            {
                switch (value)
                {
                    case null:
                        ResignedOn = null;
                        return;
                    case PresentDateLongValue:
                        ResignedOn = PresentDateValue;
                        return;
                    default:
                        ResignedOn = DateHelper.ConvertEpochToDateTime((long)value)
                            .ToString(AppConstants.FcaDateFormatFromApiResult, CultureInfo.InvariantCulture);
                        break;
                }
            }
        }
    }

    public class CompanyOfAppointment
    {
        [JsonProperty("company_name")] public string? CompanyName { get; set; }
        [JsonProperty("company_number")] public string? CompanyNumber { get; set; }
        [JsonProperty("company_status")] public string? CompanyStatus { get; set; }
    }

    public class DateData
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}