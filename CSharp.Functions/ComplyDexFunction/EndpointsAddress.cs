using Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;

namespace Api
{
    //This is all about making user input of address easy
    //like getaddress.io, etc
    public partial class Endpoints
    {
        [FunctionName(nameof(PostGetFilteredAddressAsync))]
        public async Task<IActionResult> PostGetFilteredAddressAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var payloadRaw = await new StreamReader(req.Body).ReadToEndAsync();
            var payload = JsonConvert.DeserializeObject<GetAddressPayload>(payloadRaw);

            if (payload == null)
            {
                throw new NullReferenceException($"{nameof(payload)} should not be null.");
            }

            if (string.IsNullOrEmpty(payload.Keyword))
            {
                throw new NullReferenceException($"{nameof(payload.Keyword)} should not be null.");
            }

            log.LogInformation("Called Endpoints.{PostGetFilteredAddressAsyncName} with keyword \'{PayloadKeyword}\'",
                nameof(PostGetFilteredAddressAsync), payload.Keyword);

            var result = await _getAddressService.GetFilteredAddressSuggestionAsync(payload.Keyword.ToUpper(),
                new GetAddressFilterWithResultCount
                {
                    Filter = new GetAddressFilter { Country = payload.CountryFilter ?? "" }
                });

            return new OkObjectResult(result);
        }

        // not used in frontend just testing in postman
        [FunctionName(nameof(GetFilteredAddressAsync))]
        public async Task<IActionResult> GetFilteredAddressAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{nameof(GetFilteredAddressAsync)}/{{keyword}}")]
            HttpRequest req,
            ILogger log,
            string keyword)
        {
            log.LogInformation("Called Endpoints.{FilteredAddressAsyncName} with keyword \'{Keyword}\'",
                nameof(GetFilteredAddressAsync), keyword);
            var result = await _getAddressService.GetAddressSuggestionAsync(keyword?.ToUpper());
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GetActualAddressAsync))]
        public async Task<IActionResult> GetActualAddressAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{nameof(GetActualAddressAsync)}/{{id}}")]
            HttpRequest req,
            ILogger log,
            string id)
        {
            log.LogInformation("Called Endpoints.{ActualAddressAsyncName} with id \'{Id}\'",
                nameof(GetActualAddressAsync), id);
            var result = await _getAddressService.GetActualAddressAsync(id);
            return new OkObjectResult(result);
        }
    }
}