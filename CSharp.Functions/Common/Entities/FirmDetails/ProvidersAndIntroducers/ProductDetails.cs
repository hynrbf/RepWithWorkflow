namespace Common.Entities
{
    public class ProductDetails
    {
        public IEnumerable<ProvidersProductDetails>? MortgageBroking { get; set; }
        public IEnumerable<ProvidersProductDetails>? ProtectionBroking { get; set; }
        public IEnumerable<ProvidersProductDetails>? InsuranceBroking { get; set; }
        public IEnumerable<ProvidersProductDetails>? ConsumerCredit { get; set; }
    }
}
