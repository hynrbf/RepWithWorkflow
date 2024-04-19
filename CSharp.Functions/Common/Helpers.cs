using System.Globalization;
using System.Text.RegularExpressions;

namespace Common
{
    public static class Helpers
    {
        //ToDo. remove this, do not use in Common
        //because he dont have dependency in Environment. he dont have that
        public static string GetEnvironmentVariable(string key)
            => Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);

        public static string GetOrdinal(int day)
        {
            if (day >= 11 && day <= 13)
            {
                return $"{day}th";
            }

            var lastDigit = day % 10;
            var ordinal = lastDigit switch
            {
                1 => $"{day}st",
                2 => $"{day}nd",
                3 => $"{day}rd",
                _ => $"{day}th",
            };
            return ordinal;
        }

        public static string EncodeToBase64(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }

        public static string RemovePostCodeString(this string companyName)
        {
            var firstIndex = companyName.IndexOf(" (Postcode", StringComparison.Ordinal);
            var value = firstIndex > 1 ? companyName[..firstIndex] : companyName;
            return value;
        }

        public static string CapitalizeOnlyFirstLetterOfWord(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var updatedSegments = new List<string>();
            var segments = input.Split(' ').ToList();

            foreach (var segment in segments)
            {
                var textInfo = CultureInfo.CurrentCulture.TextInfo;
                var result = textInfo.ToTitleCase(segment.ToLower());
                updatedSegments.Add(result);
            }

            return string.Join(' ', updatedSegments);
        }

        public static string RemoveNonAlphaNumeric(this string input)
        {
            // Remove all characters that are not alphabets or numerals
            return Regex.Replace(input, "[^a-zA-Z0-9]", "");
        }
    }
}