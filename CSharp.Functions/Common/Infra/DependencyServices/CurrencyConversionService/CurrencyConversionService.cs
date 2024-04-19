using Common.Entities;

namespace Common.Infra
{
    public class CurrencyConversionService : RestServiceBase, ICurrencyConversionService
    {
        private readonly string _baseUrl = AppSettingsProvider.Instance.GetValue(AppConstants.RestBaseFixerApi);
        private string _apiKey;

        public async Task<CurrencyConversion> GetCurrencyConversionLatestAsync()
        {
            var endpoint = $"{_baseUrl}/latest?access_key={_apiKey}&format=1";

            var currencyConversion = await GetRemoteAsync(endpoint, async (response) => await HandleFailureAsync<CurrencyConversion>(endpoint, response));
            return currencyConversion;
        }

        public void Register(string apiKey)
        {
            _apiKey = apiKey;
        }

        protected override HttpRequestMessage CreateRequestMessageGet(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

            request.Headers.Add("format", "1");
            request.Headers.Add("access_key", _apiKey);
            return request;
        }

        protected override HttpRequestMessage CreateRequestMessagePost(string endpoint, HttpContent httpContent)
        {
            throw new NotImplementedException();
        }
    }
}
