namespace Common.Entities
{
    public class DataProtectionLicense
    {
        public string? LicenseNumber { get; set; }

        public long? EndDate { get; set; }

        public long? RenewalDate
        {
            get
            {
                if (EndDate == null)
                {
                    return null;
                }
                var endDate = DateHelper.ConvertEpochToDateTime(EndDate.Value);
                endDate = endDate.AddDays(1);
                return DateHelper.ConvertDateTimeToEpoch(endDate);
            }
        }

        public string? DocumentUrl { get; set; }

        public bool IsForAnyOtherPurpose { get; set; }
        public bool IsUsedAutomatedProcessing { get; set; }

        public DataProtectionOfficer? DataProtectionOfficer { get; set; }

        public List<PersonalDataCategory> CategoriesOfPersonalDataService { get; set; } = new();
        public List<PersonalDataCategory> CategoriesOfPersonalDataServiceConfirmed { get; set; } = new();
        public List<PersonalDataCategory> CategoriesOfPersonalDataNonService { get; set; } = new();
        public List<PersonalDataCategory> CategoriesOfPersonalDataNonServiceConfirmed { get; set; } = new();

        public List<DataCollectionMethod> MethodsOfDataCollectionService { get; set; } = new();
        public List<DataCollectionMethod> MethodsOfDataCollectionServiceConfirmed { get; set; } = new();
        public List<DataCollectionMethod> MethodsOfDataCollectionNonService { get; set; } = new();
        public List<DataCollectionMethod> MethodsOfDataCollectionNonServiceConfirmed { get; set; } = new();

        public List<DataCollectionPurpose> PurposeOfDataCollectionService { get; set; } = new();
        public List<DataCollectionPurpose> PurposeOfDataCollectionServiceConfirmed { get; set; } = new();
        public List<DataCollectionPurpose> PurposeOfDataCollectionNonService { get; set; } = new();
        public List<DataCollectionPurpose> PurposeOfDataCollectionNonServiceConfirmed { get; set; } = new();

        public List<DataCollectionTiming> TimingOfDataCollectionServiceConfirmed { get; set; } = new();
        public List<DataCollectionTiming> TimingOfDataCollectionNonServiceConfirmed { get; set; } = new();
        public List<DataCollectionTiming> TimingOfDataCollectionService { get; set; } = new();
        public List<DataCollectionTiming> TimingOfDataCollectionNonService { get; set; } = new();
    }
}