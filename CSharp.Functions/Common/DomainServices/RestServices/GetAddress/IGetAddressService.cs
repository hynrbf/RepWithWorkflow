using Common.Entities;

namespace Common
{
    public interface IGetAddressService
    {
        Task<GetAddressSuggestion> GetAddressSuggestionAsync(string keyword);

        Task<GetAddressSuggestion> GetFilteredAddressSuggestionAsync(string keyword,
            GetAddressFilterWithResultCount filter);

        Task<GetAddressActual> GetActualAddressAsync(string id);
        void Register(string apiKey);
    }
}