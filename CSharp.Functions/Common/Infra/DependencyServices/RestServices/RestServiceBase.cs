using Newtonsoft.Json;
using Polly;
using System.Diagnostics;
using System.Text;

namespace Common.Infra
{
    public abstract class RestServiceBase
    {
        protected static string BaseApi => AppSettingsProvider.Instance.GetValue(AppConstants.RestBaseApi);

        protected RestServiceBase()
        {
            //do nothing  
        }

        protected static StringContent CastToStringContent<T>(object item)
        {
            if (item == null)
            {
                return new StringContent("");
            }

            var jsonString = JsonConvert.SerializeObject((T)item);
            var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return jsonContent;
        }

        protected async Task<T> GetRemoteAsync<T>(string endPoint,
            Func<HttpResponseMessage, Task<T>>? asyncOverrideWhenFailureAction = null)
        {
            string errorResult;

            try
            {
                var jsonResponse = await Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync
                    (
                        retryCount: 3,
                        sleepDurationProvider: _ => TimeSpan.FromSeconds(3)
                    )
                    .ExecuteAsync(async () =>
                    {
                        using var httpClient = new HttpClient();
                        var result = await httpClient.SendAsync(CreateRequestMessageGet(endPoint))
                            .ConfigureAwait(false);
                        return result;
                    });

                if (jsonResponse.IsSuccessStatusCode)
                {
                    var jsonResult = await jsonResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (string.IsNullOrWhiteSpace(jsonResult))
                    {
                        return default;
                    }

                    if (typeof(T) == typeof(string))
                    {
                        return (T)Convert.ChangeType(jsonResult, typeof(string));
                    }

                    var deserializedObject = JsonConvert.DeserializeObject<T>(jsonResult);
                    return deserializedObject;
                }

                errorResult = await jsonResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (asyncOverrideWhenFailureAction != null)
                {
                    return await asyncOverrideWhenFailureAction(jsonResponse);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(
                    $"An error was in RestServiceBase.GetRemoteAsync with message {exception.Message}");

                if (exception.InnerException != null)
                    Debug.WriteLine(
                        $"An error was in RestServiceBase.GetRemoteAsync with message {exception.InnerException.Message}");

                throw;
            }

            throw new ArgumentException(
                $"Not successful in getting results from this api {endPoint}. The response error was {errorResult}.");
        }

        protected async Task<T> PostRemoteAsync<T>(string endPoint, HttpContent httpContent)
        {
            string errorResult;

            try
            {
                var jsonResponse = await Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync
                    (
                        retryCount: 3,
                        sleepDurationProvider: _ => TimeSpan.FromSeconds(3)
                    )
                    .ExecuteAsync(async () =>
                    {
                        using var httpClient = new HttpClient();
                        var result = await httpClient.SendAsync(CreateRequestMessagePost(endPoint, httpContent))
                            .ConfigureAwait(false);
                        return result;
                    });

                if (jsonResponse.IsSuccessStatusCode)
                {
                    var jsonResult = await jsonResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (typeof(T) == typeof(string))
                        return (T)Convert.ChangeType(jsonResult, typeof(string));

                    if (!string.IsNullOrWhiteSpace(jsonResult))
                    {
                        var deserializedObject = JsonConvert.DeserializeObject<T>(jsonResult);
                        return deserializedObject;
                    }

                    errorResult = "JsonResult is null or empty for PostRemoteAsync";
                }
                else
                    errorResult = await jsonResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(
                    $"An error was in RestServiceBase.GetRemoteAsync with message {exception.Message}");

                if (exception.InnerException != null)
                    Debug.WriteLine(
                        $"An error was in RestServiceBase.GetRemoteAsync with message {exception.InnerException.Message}");

                throw;
            }

            throw new ArgumentException(
                $"Not successful in getting results from this api {endPoint}. The response error was {errorResult}.");
        }

        protected async Task PostRemoteAsync(string endPoint, HttpContent httpContent)
        {
            try
            {
                var jsonResponse = await Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync
                    (
                        retryCount: 3,
                        sleepDurationProvider: _ => TimeSpan.FromSeconds(3)
                    )
                    .ExecuteAsync(async () =>
                    {
                        using var httpClient = new HttpClient();
                        return await httpClient.SendAsync(CreateRequestMessagePost(endPoint, httpContent))
                            .ConfigureAwait(false);
                    });

                if (!jsonResponse.IsSuccessStatusCode)
                {
                    var errorResult = await jsonResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ArgumentException(
                        $"Not successful in getting results from this api {endPoint}. The response error was {errorResult}.");
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(
                    $"An error was in RestServiceBase.GetRemoteAsync with message {exception.Message}");

                if (exception.InnerException != null)
                    Debug.WriteLine(
                        $"An error was in RestServiceBase.GetRemoteAsync with message {exception.InnerException.Message}");

                throw;
            }
        }

        protected static async Task<T> HandleFailureAsync<T>(string endPoint, HttpResponseMessage response)
            where T : new()
        {
            var errorResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            //ToDo. to get back why this occurs?
            // ref : https://http.dev/509
            // Bandwidth Limit Exceeded
            if ((int)response.StatusCode == 509)
            {
                return new T();
            }

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.TooManyRequests:
                    return new T();
                case System.Net.HttpStatusCode.NotFound:
                {
                    if (errorResult.Contains("company-psc-not-found"))
                    {
                        return new T();
                    }

                    break;
                }
                case System.Net.HttpStatusCode.BadGateway:
                {
                    return new T();
                }
                default:
                    throw new ArgumentException(
                        $"Not successful in getting results from this api {endPoint}. The response error was {errorResult}.");
            }

            throw new ArgumentException(
                $"Not successful in getting results from this api {endPoint}. The response error was {errorResult}.");
        }

        protected abstract HttpRequestMessage CreateRequestMessageGet(string endpoint);
        protected abstract HttpRequestMessage CreateRequestMessagePost(string endpoint, HttpContent httpContent);
    }
}