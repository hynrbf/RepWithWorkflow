using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Common.Entities;
using System.Collections.Generic;

namespace Api
{
    //This is all about checking the company existence, registration
    //compliance, etc in trusted sources like FCA, etc
    public partial class Endpoints
    {
        #region Companies House 3rd Party Api Calls

        [FunctionName(nameof(SearchCompaniesAsync))]
        public async Task<IActionResult> SearchCompaniesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var keyword = req.Query["keyword"];
            var result = await _companiesHouseService.SearchCompaniesWithDetailsAsync(keyword);
            return new OkObjectResult(result.OrderBy(s => s.CompanyName));
        }

        /* Below Apis checks Firm Directors and Ownership e.g.
         * Corp Controller > Director(s) of SUNLIFE LIMITED
         *    /company/{{companyNumber}}/officers
         *
         * I.Ind Controller
         *   1. get controller
         *      -> GetIndividualControllersAsync
         *          -> company/10022067/persons-with-significant-control
         *   2. get company directors and director where he has been director
         *      -> GetCompanyDirectorshipAndAppointmentDetailsAsync
         *          -> /company/{{companyNumber}}/officers
         *          -> /officers/BrzLJ_Kn4Smufmmd-eK2O8s0gNM/appointments
         *   3. get company directors controlling interest
         *      -> GetCompanyControllingInterestsAsync
         *
         *  II. Corp Controller
         *   1. get controller
         *      -> GetCorporateControllersAsync
         *           -> company/10022067/persons-with-significant-control
         *   2. get individual controllers of a corporate kind, the steps in no I.
         *   3. get corporate controllers and repeat step 1 until there's no corporate controller already
         */

        [FunctionName(nameof(GetIndividualControllersAsync))]
        public async Task<IActionResult> GetIndividualControllersAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var companyNumber = req.Query["companyNo"];
            log.LogInformation(
                "{GetIndividualControllersAsync}\tGetting individual controllers for company {CompanyNumber}",
                nameof(GetIndividualControllersAsync), companyNumber);
            var response = await _companiesHouseService.GetIndividualControllersAsync(companyNumber);
            return new OkObjectResult(response);
        }

        [FunctionName(nameof(GetCorporateControllersAsync))]
        public async Task<IActionResult> GetCorporateControllersAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var companyNumber = req.Query["companyNo"];
            log.LogInformation(
                "{GetCorporateControllersAsync}\tGetting corporate controllers for company {CompanyNumber}",
                nameof(GetCorporateControllersAsync), companyNumber);
            var response = await _companiesHouseService.GetCorporateControllersRecursiveAsync(companyNumber);
            return new OkObjectResult(response);
        }

        //These below are only used for testing. The methods used here are used by the Getindividual and Get Corp endpoint above already
        [FunctionName(nameof(GetCompanyDirectorshipAndAppointmentDetailsAsync))]
        public async Task<IActionResult> GetCompanyDirectorshipAndAppointmentDetailsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var companyNumber = req.Query["companyNo"];
            log.LogInformation(
                "{CompanyDirectorshipAndAppointmentDetailsAsyncName}\tGetting company officers for company {CompanyNumber}",
                nameof(GetCompanyDirectorshipAndAppointmentDetailsAsync), companyNumber);
            var result = await _companiesHouseService.GetDirectorsAndAppointmentsAsync(companyNumber);
            return new OkObjectResult(result.OrderBy(s => s.Name));
        }

        [FunctionName(nameof(GetCompanyControllingInterestsAsync))]
        public async Task<IActionResult> GetCompanyControllingInterestsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var companyNumber = req.Query["companyNo"];
            log.LogInformation(
                "{GetCompanyControllingInterestsAsync}\tGetting company officers for company {CompanyNumber}",
                nameof(GetCompanyControllingInterestsAsync), companyNumber);
            var result = await _companiesHouseService.GetControllingInterestAsync(companyNumber);
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GetCompanyOfficersAsync))]
        public async Task<IActionResult> GetCompanyOfficersAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var companyNumber = req.Query["companyNo"];
            log.LogInformation("{CompanyOfficersAsyncName}\tGetting company officers for company {CompanyNumber}",
                nameof(GetCompanyOfficersAsync), companyNumber);
            var result = await _companiesHouseService.GetCompanyOfficersAsync(companyNumber);
            return new OkObjectResult(result.OrderBy(s => s.Name));
        }

        [FunctionName(nameof(GetCompanyActiveDirectorsAsync))]
        public async Task<IActionResult> GetCompanyActiveDirectorsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var companyNumber = req.Query["companyNo"];
            log.LogInformation(
                "{CompanyActiveDirectorsAsyncName}\tGetting company officers for company {CompanyNumber}",
                nameof(GetCompanyActiveDirectorsAsync), companyNumber);
            var result = await _companiesHouseService.GetCompanyActiveDirectorsAsync(companyNumber);
            return new OkObjectResult(result.OrderBy(s => s.Name));
        }

        [FunctionName(nameof(GetCompanyFilingHistoryAsync))]
        public async Task<IActionResult> GetCompanyFilingHistoryAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var companyNumber = req.Query["companyNo"];

            log.LogInformation("{0}\tGetting company filing history for company {1}",
                nameof(GetCompanyOfficersAsync), companyNumber);

            var result = await _companiesHouseService.GetCompanyFilingHistoryAsync(companyNumber);
            return new OkObjectResult(result.OrderByDescending(s => s.Date));
        }


        [FunctionName(nameof(GetCompanyProfileAsync))]
        public async Task<IActionResult> GetCompanyProfileAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var companyNumber = req.Query["companyNo"];
            log.LogInformation("{CompanyProfileAsyncName}\\tGetting company profile for company {CompanyNumber}",
                nameof(GetCompanyProfileAsync), companyNumber);
            var result = await _companiesHouseService.GetCompanyProfileAsync(companyNumber);
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GetAllCompanyHouseStatusesAsync))]
        public async Task<IActionResult> GetAllCompanyHouseStatusesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var permissionResult = await _companyHouseRepository.GetAllCompanyHouseStatusesAsync();
            return new OkObjectResult(permissionResult);
        }

        #endregion

        #region Fca Services 3rd Party Api Calls

        [FunctionName(nameof(GetFirmIndividualsAsync))]
        public async Task<IActionResult> GetFirmIndividualsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var firmReferenceNo = req.Query["firmRefNo"];
            var result = await _fcaService.GetFirmIndividualsAsync(firmReferenceNo);
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GetMatchedFirmsAsync))]
        public async Task<IActionResult> GetMatchedFirmsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var companyName = req.Query["q"];
            var companyNumber = req.Query["companyNo"];
            var companyAddress = req.Query["companyAddress"];
            var aFirm = req.Query["isFirm"];
            var isFirm = false;

            if (bool.TryParse(aFirm, out var aFirmResult))
            {
                isFirm = aFirmResult;
            }

            var result = await _fcaService.SearchMatchedFirmsAsync(companyName, isFirm,
                companyNumber, companyAddress);
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GetFirmPermissionsAsync))]
        public async Task<IActionResult> GetFirmPermissionsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetFirmPermissionsAsync)}/{{firmRefNo}}")]
            HttpRequest req,
            ILogger log,
            string firmRefNo)
        {
            var result = await _fcaService.SearchFirmPermissionsAsync(firmRefNo);
            var clientMoney = await _fcaService.SearchFirmClientMoneyPermissionAsync(firmRefNo);

            if (!string.IsNullOrEmpty(clientMoney))
            {
                result.PermissionNames.Add(clientMoney);
            }

            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GetCompanyPermissionsAsync))]
        public async Task<IActionResult> GetCompanyPermissionsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetCompanyPermissionsAsync)}/{{firmRefNo}}")]
            HttpRequest req,
            ILogger log,
            string firmRefNo)
        {
            var result = await _fcaService.GetFirmPermissionsAsync(firmRefNo);
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GetIndividualControlledFunctionsAsync))]
        public async Task<IActionResult> GetIndividualControlledFunctionsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetIndividualControlledFunctionsAsync)}/{{individualRefNo}}")]
            HttpRequest req,
            ILogger log,
            string individualRefNo)
        {
            log.LogInformation(
                "Api.{GetIndividualControlledFunctionsAsync} called with individual reference number \'{IndividualRefNo}\'",
                nameof(GetIndividualControlledFunctionsAsync), individualRefNo);
            var result = await _fcaService.GetIndividualControlledFunctionsAsync(individualRefNo);
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GetFcaFirmDetailsAsync))]
        public async Task<IActionResult> GetFcaFirmDetailsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetFcaFirmDetailsAsync)}/{{firmRefNo}}")]
            HttpRequest req,
            ILogger log,
            string firmRefNo)
        {
            var firmDetails = await _fcaService.SearchFcaFirmDetailsAsync(firmRefNo);
            return new OkObjectResult(firmDetails);
        }

        [FunctionName(nameof(GetFirmNameDetailsAsync))]
        public async Task<IActionResult> GetFirmNameDetailsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetFirmNameDetailsAsync)}/{{firmRefNo}}")]
            HttpRequest req,
            ILogger log,
            string firmRefNo)
        {
            log.LogInformation("Api.{FirmNameDetailsAsyncName} called with firm reference number \'{FirmRefNo}\'",
                nameof(GetFirmNameDetailsAsync), firmRefNo);
            var tradingNames = await _fcaService.GetTradingNamesAsync(firmRefNo);
            return new OkObjectResult(tradingNames);
        }

        [FunctionName(nameof(GetFirmAddressDetailsAsync))]
        public async Task<IActionResult> GetFirmAddressDetailsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetFirmAddressDetailsAsync)}/{{firmRefNo}}")]
            HttpRequest req,
            ILogger log,
            string firmRefNo)
        {
            var addressType = req.Query["type"];
            log.LogInformation(
                "Api.{FirmNameDetailsAsyncName} called with firm reference number \'{FirmRefNo}\' {Empty}",
                nameof(GetFirmNameDetailsAsync), firmRefNo,
                (string.IsNullOrEmpty(addressType) ? string.Empty : $" with address type '{addressType}'"));
            var fcaAddresses = await _fcaService.GetFirmAddressDetailsAsync(firmRefNo, addressType);
            return new OkObjectResult(fcaAddresses);
        }

        [FunctionName(nameof(GetMatchedCompanyAsync))]
        public async Task<IActionResult> GetMatchedCompanyAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var companyNumber = req.Query["companyNo"];
            var companyName = req.Query["companyName"];
            var fcaAddresses = await _fcaService.GetMatchedCompanyAsync(companyName, companyNumber);
            return new OkObjectResult(fcaAddresses);
        }

        [FunctionName(nameof(GetFcaAppointedRepresentativesAsync))]
        public async Task<IActionResult> GetFcaAppointedRepresentativesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var firmRefNo = req.Query["firmRefNo"];

            //both full and introducer are already for AR, that's why GetIntroducer below is intentionally 'NA' so it will return nothing
            var taskFull = await _fcaService.GetAppointedRepresentativesAsync(firmRefNo, "Full");
            var taskIntroducer = await _fcaService.GetAppointedRepresentativesAsync(firmRefNo, "Introducer");

            // Intentionally remove Task.WhenAll as this won't benefit from Cache
            var mergedResults = taskFull.Concat(taskIntroducer);

            return new OkObjectResult(mergedResults);
        }

        [FunctionName(nameof(GetIntroducersAsync))]
        public async Task<IActionResult> GetIntroducersAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var firmRefNo = req.Query["firmRefNo"];
            var results = await _fcaService.GetAppointedRepresentativesAsync(firmRefNo, "NA");
            return new OkObjectResult(results);
        }

        [FunctionName(nameof(GetProvidersAsync))]
        public async Task<IActionResult> GetProvidersAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var firmRefNo = req.Query["firmRefNo"];
            var results = await _fcaService.GetAppointedRepresentativesAsync(firmRefNo, "Provider");
            return new OkObjectResult(results);
        }

        [FunctionName(nameof(GetFirmPraStatusAsync))]
        public async Task<IActionResult> GetFirmPraStatusAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var firmReferenceNo = req.Query["firmRefNo"];
            var result = await _fcaService.GetFirmRegulatorsAsync(firmReferenceNo);

            const string praRegulatorName = "Prudential Regulation Authority";
            var praStatus = "Not Authorised";

            var regulators = result.ToList();
            if (!regulators.Any())
            {
                return new OkObjectResult(praStatus);
            }

            var validPraRegulator = regulators.FirstOrDefault(r =>
                r.Name != null &&
                r.Name.Trim().Equals(praRegulatorName, StringComparison.InvariantCultureIgnoreCase) &&
                string.IsNullOrEmpty(r.TerminationDate));
            if (validPraRegulator != null)
            {
                praStatus = "Authorised";
            }

            return new OkObjectResult(praStatus);
        }

        [FunctionName(nameof(SearchFirmsByFirmNameKeywordAsync))]
        public async Task<IActionResult> SearchFirmsByFirmNameKeywordAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var keyword = req.Query["keyword"];

            if (string.IsNullOrEmpty(keyword))
            {
                return new OkObjectResult(new List<Company>());
            }

            var firms = await _fcaService.SearchFirmsByFirmNameKeywordAsync(keyword);
            return new OkObjectResult(firms);
        }

        #endregion

        #region ICO 3rd Party API Calls

        [FunctionName(nameof(SearchIcoDetailsAsync))]
        public async Task<IActionResult> SearchIcoDetailsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var searchInput = new IcoSearchInput
            {
                CompanyName = req.Query["companyName"],
                Reference = req.Query["reference"],
                Address = req.Query["address"],
                Postcode = req.Query["postcode"],
            };

            log.LogInformation("{SearchIcoDetailsAsync}\tGetting ICO details for company {CompanyName}",
                nameof(GetIndividualControllersAsync), searchInput.CompanyName);

            return !string.IsNullOrEmpty(searchInput.Reference)
                ? new OkObjectResult(await _icoService.GetDetailsByReferenceAsync(searchInput.Reference))
                : new OkObjectResult(await _icoService.SearchDetailsAsync(searchInput));
        }

        #endregion

        #region Ratings API

        [FunctionName(nameof(GetInsurerRatingsAsync))]
        public async Task<IActionResult> GetInsurerRatingsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var companyName = req.Query["company"];
            if (string.IsNullOrEmpty(companyName))
            {
                return new OkObjectResult(new InsurerRating());
            }

            var ratings = await _ratingsService.GetInsurerRatingAsync(companyName);
            return new OkObjectResult(ratings);
        }

        #endregion
    }
}