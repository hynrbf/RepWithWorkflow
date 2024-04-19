using System;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using Common.Entities;
using Common.Infra;

namespace Api.Infra
{
    public class GetAddressService : RestServiceBase, IGetAddressService
    {
        private readonly string _baseUrl = AppSettingsProvider.Instance.GetValue(AppConstants.GetAddressBaseApi);
        private string? _apiKey;

        public async Task<GetAddressActual> GetActualAddressAsync(string id)
        {
            var endpoint = $"{_baseUrl}/get/{id}?api-key={_apiKey}";
            var result = await GetRemoteAsync<GetAddressActual>(endpoint);
            return result;
        }

        public async Task<GetAddressSuggestion> GetAddressSuggestionAsync(string keyword)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new NullReferenceException("api key missing");
            }

            var endpoint = $"{_baseUrl}/autocomplete/{keyword}?api-key={_apiKey}";
            var result = await GetRemoteAsync<GetAddressSuggestion>(endpoint);
            return result;
        }

        public async Task<GetAddressSuggestion> GetFilteredAddressSuggestionAsync(string keyword,
            GetAddressFilterWithResultCount filter)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new NullReferenceException("api key missing");
            }

            var endpoint = $"{_baseUrl}/autocomplete/{keyword}?api-key={_apiKey}";
            var content = CastToStringContent<GetAddressFilterWithResultCount>(filter);
            var result = await PostRemoteAsync<GetAddressSuggestion>(endpoint, content);
            return result;
        }

        public void Register(string apiKey)
        {
            _apiKey = apiKey;
        }

        protected override HttpRequestMessage CreateRequestMessageGet(string endpoint)
            => new HttpRequestMessage(HttpMethod.Get, endpoint);

        protected override HttpRequestMessage CreateRequestMessagePost(string endpoint, HttpContent httpContent)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Content = httpContent;
            return request;
        }
    }
}