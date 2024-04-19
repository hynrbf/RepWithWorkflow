namespace Common.Entities
{
    public class ArActivity
    {
        public string? Id { get; set; }
        public string? ProductId { get; set; }
        public Money? AnnualFeeIncome { get; set; }
        public Money? AnnualCommissionIncome { get; set; }
        public string? Limitations { get; set; }
        public int? YearsOfExperience {  get; set; }
        public string? Name { get; set; }
        public bool IsAppointed { get; set; }
        public bool HasLimitation { get; set; }
        public bool? HasPendingApplication { get; set; }
        public bool? IsModified { get; set; }
        public bool? IsNewProduct { get; set; }
        public Money? ProjectedAnnualFee { get; set; }
        public Money? ProjectedAnnualCommissionIncome { get; set; }
    }
}
