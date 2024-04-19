namespace Common.Entities
{
    public class RatingResult<T>
    {
        public string IssuerName { get; set; } = "";

        public string? RatingType { get; set; }

        public string? RatingValue { get; set; }

        public T? Rating { get; set; }

        public DateTime? RatingDate { get; set; }
    }
}