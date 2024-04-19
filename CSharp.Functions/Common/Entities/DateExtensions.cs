namespace Common.Entities;

public static class DateExtensions
{
    public static long ConvertToEpoch(this DateTime dateTime)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return Convert.ToInt64((dateTime - epoch).TotalSeconds);
    }
}