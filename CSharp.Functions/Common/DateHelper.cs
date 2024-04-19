namespace Common
{
    public static class DateHelper
    {
        public static DateTime ConvertEpochToDateTime(long epoch)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(epoch);
        }

        public static long ConvertDateTimeToEpoch(DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((dateTime - epoch).TotalSeconds);
        }

        public static long GetCurrentDateTimeInEpoch()
        {
            var currentTime = DateTime.UtcNow;
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((currentTime - epoch).TotalSeconds);
        }
    }
}