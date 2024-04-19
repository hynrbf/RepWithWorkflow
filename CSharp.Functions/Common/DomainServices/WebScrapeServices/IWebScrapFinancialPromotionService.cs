namespace Common
{
    public interface IWebScrapFinancialPromotionService
    {
        Task RunWebScrapToFinancialPromotion(List<string> namesForWarning);
    }
}