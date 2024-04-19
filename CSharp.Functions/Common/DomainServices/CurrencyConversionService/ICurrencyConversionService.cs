using Common.Entities;

namespace Common
{
    public interface ICurrencyConversionService
    {
        Task<CurrencyConversion> GetCurrencyConversionLatestAsync();

        void Register(string apiKey);
    }
}
