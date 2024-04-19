using Common.Entities;

namespace Common
{
    public interface IAuth0Service
    {
        Task<AccessToken> GetAccessTokenForSignUpAsync();
        Task<Auth0User?> CreateOnboardingUserAsync(OnboardingUserBase onboardingUser);
        Task<bool> DeleteUserAsync(string userId);
        Task<HttpResponseMessage> ChangePasswordAsync(string email, string newPassword);
        void Register(string clientId, string clientSecret, string baseUrl, string apiToken);
    }
}
