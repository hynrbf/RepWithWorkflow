using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Common.Entities;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Infra;
using Newtonsoft.Json;

namespace Api.Infra
{
    public class Auth0Service : RestServiceBase, IAuth0Service
    {
        private const string Connection = "Username-Password-Authentication";

        #region Signup credential

        // TODO. To move below constants to Vault soon.
        // should be change to 'signup@suntech.gi'
        private const string SignUpUserName = "chito@suntech.gi";
        private const string SignUpPassword = "Grace28%";

        // TODO. how to generate this. programmatically
        // this expires 30 days, so we will get back to this on how we get refresh token
        private string? _apiAccessToken;

        #endregion

        private string? _clientId;
        private string? _clientSecret;
        private string? _baseUrl;

        public void Register(string clientId, string clientSecret, string baseUrl, string apiToken)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _baseUrl = baseUrl;
            _apiAccessToken = apiToken;
        }

        public async Task<Auth0User> CreateOnboardingUserAsync(
            OnboardingUserBase onboardingUser)
        {
            if (string.IsNullOrEmpty(onboardingUser?.Email))
            {
                throw new Exception(
                    $"Email Address should not be empty at {nameof(Auth0Service)}.{nameof(CreateOnboardingUserAsync)}");
            }

            if (string.IsNullOrEmpty(_apiAccessToken))
            {
                throw new NullReferenceException("The api token of Auth should not be null. Call register first.");
            }

            // Check if user is existing
            var existingUser = await GetUserByEmailAsync(onboardingUser.Email);

            if (existingUser != null)
            {
                return existingUser;
            }

            var onboardingUserId = string.IsNullOrEmpty(onboardingUser.Id)
                ? Guid.NewGuid().ToString()
                : onboardingUser.Id;
            return await CreateAuthUserAsync(onboardingUserId, onboardingUser);
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            userId = userId.Replace("-", string.Empty);
            userId = $"auth0|{userId}";
            var endpoint = $"{_baseUrl}/api/v2/users/{Uri.EscapeDataString(userId)}";
            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            request.Headers.Add("Authorization", $"Bearer {_apiAccessToken}");
            var httpClient = new HttpClient();
            var result = await httpClient.SendAsync(request);
            var status = (int)result.StatusCode;
            return status is >= 200 and <= 204;
        }

        public async Task<AccessToken> GetAccessTokenForSignUpAsync()
        {
            if (string.IsNullOrEmpty(_baseUrl))
            {
                throw new NullReferenceException("The base url of Auth should not be null");
            }

            var endpoint = $"{_baseUrl}/oauth/token";
            var result = await PostRemoteAsync<AccessToken>(endpoint, new StringContent(""));
            return result;
        }

        public async Task<HttpResponseMessage> ChangePasswordAsync(string email, string newPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentException("Email and Password are required");
            }

            var user = await GetUserByEmailAsync(email);

            if (user == null)
            {
                throw new NullReferenceException("User is not found");
            }

            var userJson = JsonConvert.SerializeObject(new
            {
                password = newPassword,
                connection = Connection
            });

            var endpoint = $"{_baseUrl}/api/v2/users/{user.Id}";
            var request = new HttpRequestMessage(HttpMethod.Patch, endpoint);
            request.Headers.Add("Authorization", $"Bearer {_apiAccessToken}");
            request.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
            var httpClient = new HttpClient();
            var result = await httpClient.SendAsync(request);
            return result;
        }

        protected override HttpRequestMessage CreateRequestMessageGet(string endpoint)
        {
            throw new NotImplementedException();
        }

        protected override HttpRequestMessage CreateRequestMessagePost(string endpoint, HttpContent httpContent)
        {
            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
            {
                throw new NullReferenceException("The clientid and client secret of Auth should not be null");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            var collection = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "password"),
                new("client_id", _clientId),
                new("client_secret", _clientSecret),
                new("username", SignUpUserName),
                new("password", SignUpPassword)
            };
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            return request;
        }

        private async Task<Auth0User?> GetUserByEmailAsync(string email)
        {
            var endpoint = $"{_baseUrl}/api/v2/users-by-email?email={email}";
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Add("Authorization", $"Bearer {_apiAccessToken}");
            var httpClient = new HttpClient();
            var result = await httpClient.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();
            var existingUsers = JsonConvert.DeserializeObject<List<Auth0User>>(response);
            return existingUsers?.FirstOrDefault();
        }

        private async Task<Auth0User> CreateAuthUserAsync(string onboardingUserId,
            OnboardingUserBase onboardingUser)
        {
            var auth0User = new CreateAuth0UserRequest
            {
                Id = onboardingUserId.Replace("-", string.Empty),
                Email = onboardingUser.Email,
                FirstName = onboardingUser.FirstName,
                LastName = onboardingUser.LastName,
                DisplayName = onboardingUser.Email,
                Connection = Connection,
                Password = onboardingUser.TempPassword
            };

            var userJson = JsonConvert.SerializeObject(auth0User);
            var endpoint = $"{_baseUrl}/api/v2/users";
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add("Authorization", $"Bearer {_apiAccessToken}");
            request.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
            var httpClient = new HttpClient();
            var result = await httpClient.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Auth0User>(response);
        }
    }
}