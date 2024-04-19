using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using Common.Entities;
using Newtonsoft.Json;
using System.IO;
using Common;

namespace Api
{
    public partial class Endpoints
    {
        [FunctionName(nameof(GetCurrencyConversions))]
        public async Task<IActionResult> GetCurrencyConversions(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var result = await _currencyConversionService.GetCurrencyConversionLatestAsync();
            result.DateTimestamp = GetDateTimeFormat(result.Timestamp);
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GetCurrencyConversionByDateTime))]
        public async Task<IActionResult> GetCurrencyConversionByDateTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var dateTime = req.Query["dateTime"].ToString();
            var result = await _currencyConversionRepository.GetCurrencyConversionByDateTime(dateTime);
            //result.DateTimestamp = GetDateTimeFormat(result.Timestamp);
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(SaveCurrencyConversion))]
        public async Task<IActionResult> SaveCurrencyConversion(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var currencyConversion = JsonConvert.DeserializeObject<CurrencyConversion>(requestBody);
            currencyConversion.DateTimestamp = GetDateTimeFormat(currencyConversion.Timestamp);
            var result = await _currencyConversionRepository.SaveCurrencyConversionAsync(currencyConversion);
            return new OkObjectResult(result);
        }

        #region Private Methods

        private static string GetDateTimeFormat(long timestamp)
        {
            var readableDateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
            var dateTimestamp = readableDateTime.ToString(AppConstants.DateTimeFormatForCurrencyConversion);
            return dateTimestamp;
        }

        #endregion
    }
}