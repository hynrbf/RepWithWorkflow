using Common.Entities;

namespace Common
{
    public interface ICurrencyConversionRepository
    {
        Task<CurrencyConversion> SaveCurrencyConversionAsync(CurrencyConversion customer);
        Task<CurrencyConversion?> GetCurrencyConversionByDateTime(string dateTime);
        Task<CurrencyConversion> GetCurrencyConversionByIdAsync(string id);
    }
}
