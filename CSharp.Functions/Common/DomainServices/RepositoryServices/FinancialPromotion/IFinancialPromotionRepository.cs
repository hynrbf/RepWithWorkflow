using Common.Entities;

namespace Common
{
    public interface IFinancialPromotionRepository
    {
        Task<IEnumerable<FinancialPromotion>> GetAllFinancialPromotionsAsync();
        Task<IEnumerable<FinancialPromotion>> GetFinancialPromotionsAsync(string customerId);
        Task<FinancialPromotion?> GetFinancialPromotionByIdAsync(string id);
        Task<FinancialPromotion> SaveOrUpdateFinancialPromotionAsync(FinancialPromotion employee);
        Task<bool> DeleteFinancialPromotionByIdAsync(string id);
        Task<bool> DeleteFinancialPromotionsAsync(string customerId);
    }
}