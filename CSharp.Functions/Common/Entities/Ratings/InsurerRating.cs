namespace Common.Entities
{
    /// <summary>
    /// Effective (lowest) rating value, if company has multiple ratings
    /// </summary>
    public class InsurerRating
    {
        public string Rating { get; set; }

        public string Issuer { get; set; }
    }
}