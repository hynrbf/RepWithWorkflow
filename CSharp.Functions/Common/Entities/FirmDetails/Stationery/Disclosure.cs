namespace Common.Entities
{
    public class Disclosure
    {
        public List<string>? TimePeriodDisclosureText { get; set; }
        public List<string>? TimePeriodDisclosureConfirmedText { get; set; }

        public List<string>? AffiliateDisclosureText { get; set; }
        public List<string>? AffiliateDisclosureConfirmedText { get; set; }

        public List<string>? TaxDisclosureText { get; set; }
        public List<string>? TaxDisclosureConfirmedText { get; set; }

        public List<string>? MortgageDisclosureText { get; set; }
        public List<string>? MortgageDisclosureConfirmedText { get; set; }

        public List<string>? InvestmentDisclosureText { get; set; }
        public List<string>? InvestmentDisclosureConfirmedText { get; set; }

        public List<string>? CryptoDisclosureText { get; set; }
        public List<string>? CryptoDisclosureConfirmedText { get; set; }
    }
}