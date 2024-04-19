namespace Common
{
    public static class AppConstants
    {
        //Keys
        public const string RestBaseApi = "RestBaseApi";
        public const string RestBaseFcaApi = "RestBaseFcaApi";
        public const string RestBaseCompanyHouseApi = "RestBaseCompanyHouseApi";
        public const string RestBaseIcoApi = "RestBaseIcoApi";
        public const string RestBaseFixerApi = "RestBaseFixerApi";
        public const string CalendlyBaseApi = "CalendlyBaseApi";
        public const string GetAddressBaseApi = "GetAddressBaseApi";
        public const string PuppeteerScraperApi = "PuppeteerScraperApi";
        public const string InsurerRatingsApi = "InsurerRatingsApi";

        //cache keys
        public const int MemoryCacheOneDayDurationInMinutes = 1440;
        public const int MemoryCacheOneHourInMinutes = 60;

        //Misc
        public const string NotApplicable = "Not Applicable";
        public const int DefaultItemsPerPageNoUsedInFcaOrCh = 20;
        public const string FcaDateFormatFromApiResult = "yyyy-MM-dd";
        public const string DateFormatDefault = "dd MMMM, yyyy";
        public const string TimeFormatDefault = "HH:mm:ss";
        public const string DateTimeFormatForCurrencyConversion = "yyyy-MM-dd HH:mm";
    }
}