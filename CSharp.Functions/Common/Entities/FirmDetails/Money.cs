namespace Common.Entities
{
    public class Money
    {
        public string Currency { get; set; } = "GBP";
        public string Symbol { get; set; } = "£";
        public decimal? Amount { get; set; }
    }
}
