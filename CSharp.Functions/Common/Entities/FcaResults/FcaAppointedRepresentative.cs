using Common.Infra;
using Newtonsoft.Json;
using System.Globalization;

namespace Common.Entities
{
    public class FcaAppointedRepresentative
    {
        private const string FcaDateFormat = "dd/MM/yyyy";  // e.g. 30/09/2020

        [JsonProperty("URL")] public string? FcaUrl { get; set; }
        [JsonProperty("Termination Date")] public string? FcaTerminationDate { get; set; }
        [JsonProperty("RecordSubType")] public string? RecordSubType { get; set; }
        [JsonProperty("Principal FRN")] public string? FcaPrincipalFrn { get; set; }
        [JsonProperty("Principal Firm Name")] public string? FcaPrincipalFirmName { get; set; }
        [JsonProperty("Effective Date")] public string? FcaEffectiveDate { get; set; }
        [JsonProperty("EEA Tied Agent")] public string? FcaEeaTiedAgent { get; set; }
        [JsonProperty("Tied Agent")] public string? FcaTiedAgent { get; set; }
        [JsonProperty("FRN")] public string? FirmReferenceNumber { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Name { get; set; }

        public string? Frn => FirmReferenceNumber;

        public bool IsCurrentRepresentative { get; set; } = true;

        public string? PrincipalFirmName => FcaPrincipalFirmName;

        public string? PrincipalFrn => FcaPrincipalFrn;

        public string? Url => FcaUrl;

        public bool IsEeaTiedAgent => bool.Parse(FcaEeaTiedAgent ?? "false");

        public bool IsTiedAgent => bool.Parse(FcaTiedAgent ?? "false");

        public string? TerminationDateStr => FcaTerminationDate;

        public long? TerminationDate
        {
            get
            {
                if (string.IsNullOrEmpty(FcaTerminationDate))
                {
                    return null;
                }

                var dateTimeObj = DateTime.ParseExact(FcaTerminationDate, FcaDateFormat, CultureInfo.InvariantCulture);
                return DateHelper.ConvertDateTimeToEpoch(dateTimeObj);
            }
        }

        public string? EffectiveDateStr => FcaEffectiveDate;

        public long? EffectiveDate
        {
            get
            {
                if (string.IsNullOrEmpty(FcaEffectiveDate))
                {
                    return null;
                }

                var dateTimeObj = DateTime.ParseExact(FcaEffectiveDate, FcaDateFormat, CultureInfo.InvariantCulture);
                return DateHelper.ConvertDateTimeToEpoch(dateTimeObj);
            }
        }
    }
}