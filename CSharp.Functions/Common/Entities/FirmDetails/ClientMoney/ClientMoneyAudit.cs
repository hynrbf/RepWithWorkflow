namespace Common.Entities
{
    public class ClientMoneyAudit
    {
        public decimal HighestAmount1 { get; set; }
        public decimal HighestAmount1From { get; set; }
        public decimal HighestAmount1To { get; set; }
        public decimal HighestAmount2 { get; set; }
        public decimal HighestAmount2From { get; set; }
        public bool HasObtainedInLastYear { get; set; }
        public long ReviewPeriodStartDate { get; set; }
        public long ReviewPeriodEndtDate { get; set; }
        public long ReportDate { get; set; }
        public bool UndertakeClientAccountRecon { get; set; }
    }
}
