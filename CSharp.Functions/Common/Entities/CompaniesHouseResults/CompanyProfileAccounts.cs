using Newtonsoft.Json;
using System.Globalization;

namespace Common.Entities
{
    public class CompanyProfileAccounts
    {
        [JsonProperty("last_accounts")] public LastAccounts? LastAccounts { get; set; }
        [JsonProperty("accounting_reference_date")]
        public AccountingReferenceDate? AccountingReferenceDate { get; set; }
    }

    public class LastAccounts
    {
        private const string CompanyHouseDateFormat = "yyyy-MM-dd";

        [JsonProperty("period_start_on")] public string? PeriodStartOnStr { get; set; }
        [JsonProperty("period_end_on")] public string? PeriodEndOnStr { get; set; }
        [JsonProperty("made_up_to")] public string? MadeUpToStr { get; set; }
        public string? Type { get; set; }

        public long? PeriodStartOn => ConvertDateTimeToEpoch(PeriodStartOnStr);

        public long? PeriodEndOn => ConvertDateTimeToEpoch(PeriodEndOnStr);

        public long? MadeUpTo => ConvertDateTimeToEpoch(MadeUpToStr);

        private static long? ConvertDateTimeToEpoch(string? dateString)
        {
            if (!DateTime.TryParseExact(dateString, CompanyHouseDateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var dateValue))
            {
                return null;
            }

            return DateHelper.ConvertDateTimeToEpoch(dateValue);
        }
    }

    public class AccountingReferenceDate
    {
        public string? Day { get; set; }
        public string? Month { get; set; }
    }
}
