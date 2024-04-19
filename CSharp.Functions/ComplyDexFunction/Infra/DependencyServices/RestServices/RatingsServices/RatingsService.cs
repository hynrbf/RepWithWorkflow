using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Common;
using Common.Entities;
using Common.Infra;

namespace Api.Infra
{
    public class RatingsService : RestServiceBase, IRatingsService
    {
        private readonly string _baseUrl = AppSettingsProvider.Instance.GetValue(AppConstants.InsurerRatingsApi);
        private readonly MemoryCache _memoryCache = MemoryCache.Default;

        public async Task<InsurerRating> GetInsurerRatingAsync(string companyName)
        {
            var fitchRatingTask = GetFitchRatingAsync(companyName);
            var spRatingTask = GetSPRatingAsync(companyName);
            var moodysRatingTask = GetMoodysRatingAsync(companyName);

            await Task.WhenAll(fitchRatingTask, spRatingTask, moodysRatingTask);
            var issuerRatings = new List<dynamic>
            {
                fitchRatingTask.Result,
                spRatingTask.Result,
                moodysRatingTask.Result
            };

            //Select the lowest rating if has multiple ratings
            var selectedIssuerRating = issuerRatings.MaxBy(r => (int)r.Rating);

            return new InsurerRating
                { Issuer = selectedIssuerRating.IssuerName, Rating = selectedIssuerRating.RatingValue };
        }

        protected override HttpRequestMessage CreateRequestMessageGet(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            return request;
        }

        protected override HttpRequestMessage CreateRequestMessagePost(string endpoint, HttpContent httpContent)
        {
            throw new NotImplementedException();
        }

        private async Task<RatingResult<FitchRating>> GetFitchRatingAsync(string companyName)
        {
            var endpoint = $"{_baseUrl}/fitch/?company={companyName}";
            if (_memoryCache.Get(endpoint) is RatingResult<FitchRating> cachedValue)
            {
                return cachedValue;
            }

            var output = await GetRemoteAsync(endpoint,
                async response => await HandleFailureAsync<RatingResult<FitchRating>>(endpoint, response));
            output.Rating = GetEnumFromNameOrDesc<FitchRating>(output.RatingValue) ?? FitchRating.NotRated;
            output.IssuerName = "Fitch Ratings";

            _memoryCache.AddOrGetExisting(endpoint, output,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return output;
        }

        private async Task<RatingResult<SPRating>> GetSPRatingAsync(string companyName)
        {
            var endpoint = $"{_baseUrl}/spglobal/?company={companyName}";
            if (_memoryCache.Get(endpoint) is RatingResult<SPRating> cachedValue)
            {
                return cachedValue;
            }

            var output = await GetRemoteAsync(endpoint,
                async response => await HandleFailureAsync<RatingResult<SPRating>>(endpoint, response));
            output.Rating = GetEnumFromNameOrDesc<SPRating>(output.RatingValue) ?? SPRating.NotRated;
            output.IssuerName = "S&P Ratings";

            _memoryCache.AddOrGetExisting(endpoint, output,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return output;
        }

        private async Task<RatingResult<MoodysRating>> GetMoodysRatingAsync(string companyName)
        {
            var endpoint = $"{_baseUrl}/moodys/?company={companyName}";
            if (_memoryCache.Get(endpoint) is RatingResult<MoodysRating> cachedValue)
            {
                return cachedValue;
            }

            var output = await GetRemoteAsync(endpoint,
                async response => await HandleFailureAsync<RatingResult<MoodysRating>>(endpoint, response));
            output.Rating = GetEnumFromNameOrDesc<MoodysRating>(output.RatingValue) ?? MoodysRating.NotRated;
            output.IssuerName = "Moody's Investor Services";

            _memoryCache.AddOrGetExisting(endpoint, output,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return output;
        }

        private static T? GetEnumFromNameOrDesc<T>(string input) where T : struct, Enum
        {
            return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => new
                {
                    Field = f,
                    Attribute = f.GetCustomAttribute<DescriptionAttribute>()
                })
                .Where(f => string.Equals(f.Field.Name, input, StringComparison.OrdinalIgnoreCase)
                            || (f.Attribute != null && string.Equals(f.Attribute.Description, input,
                                StringComparison.OrdinalIgnoreCase)))
                .Select(f => (T?)f.Field.GetValue(null))
                .FirstOrDefault();
        }
    }
}