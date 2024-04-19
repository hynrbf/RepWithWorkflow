using Common.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Caching;
using System.Web;

namespace Common.Infra
{
    public class FcaService : RestServiceBase, IFcaService
    {
        private readonly IServiceMapper _serviceMapper;
        private readonly IBaseFirmPermissionRepository _baseFirmPermissionRepository;
        private readonly IFcaRoleRepository _fcaRoleRepository;
        private readonly bool _isFcaSearchDetailEnabled;
        private readonly bool _isGetFirmRefNoForDirectorship;
        private readonly string _baseUrl = AppSettingsProvider.Instance.GetValue(AppConstants.RestBaseFcaApi);
        private readonly MemoryCache _memoryCache;

        public FcaService(IBaseFirmPermissionRepository baseFirmPermissionRepository,
            IFcaRoleRepository fcaRoleRepository)
        {
            _baseFirmPermissionRepository = baseFirmPermissionRepository;
            _fcaRoleRepository = fcaRoleRepository;
            _serviceMapper = new ServiceMapper();
            var fcaSearchDetailEnable =
                Environment.GetEnvironmentVariable("FcaSearchDetailEnabled", EnvironmentVariableTarget.Process);

            if (!bool.TryParse(fcaSearchDetailEnable, out var isFcaSearchDetailEnable))
            {
                _memoryCache = MemoryCache.Default;
                return;
            }

            _isFcaSearchDetailEnabled = isFcaSearchDetailEnable;

            var getFirmRefNoForDirectorship =
                Environment.GetEnvironmentVariable("IsGetFrnForDirectorships", EnvironmentVariableTarget.Process);

            if (!bool.TryParse(getFirmRefNoForDirectorship, out var isGetFirmRefNoForDirectorship))
            {
                _memoryCache = MemoryCache.Default;
                return;
            }

            _isGetFirmRefNoForDirectorship = isGetFirmRefNoForDirectorship;
            _memoryCache = MemoryCache.Default;
        }

        public async Task<FcaPermissionsResult> SearchFirmPermissionsAsync(string firmRefNo)
        {
            var endpoint = $"{_baseUrl}/services/V0.1/Firm/{firmRefNo}/Permissions";
            if (_memoryCache.Get(endpoint) is FcaPermissionsResult cachedValue)
            {
                return cachedValue;
            }

            var result = await GetRemoteAsync(endpoint,
                async response => await HandleFailureAsync<FcaCompanyPermissionsResult>(endpoint, response));

            if (result.Data == null)
            {
                return new FcaPermissionsResult();
            }

            if (result.Data is not JObject resultJObject)
            {
                return new FcaPermissionsResult();
            }

            var items = resultJObject.AsJEnumerable();
            var permissionLists = (from item in items
                let name = (item as JProperty)?.Name
                select name).ToList();

            var fcaPermissionsResult = new FcaPermissionsResult
            {
                PermissionNames = permissionLists,
                Raw = JsonConvert.SerializeObject(items)
            };

            _memoryCache.AddOrGetExisting(endpoint, fcaPermissionsResult,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return fcaPermissionsResult;
        }

        public async Task<List<FirmPermission>> GetFirmPermissionsAsync(string firmRefNo)
        {
            var endpoint = $"{_baseUrl}/services/V0.1/Firm/{firmRefNo}/Permissions";
            var result = await GetRemoteAsync(endpoint,
                async (response) => await HandleFailureAsync<FcaCompanyPermissionsResult>(endpoint, response));

            if (result.Data == null)
            {
                return new List<FirmPermission>();
            }

            if (result.Data is not JObject resultJObject)
            {
                return new List<FirmPermission>();
            }

            var fcPermissionItems = resultJObject.AsJEnumerable();

            var baseFirmPermissions = (await _baseFirmPermissionRepository.GetBaseFirmPermissionsAsync()).ToList();

            var firmPermissions = new List<FirmPermission>();
            foreach (var fcaPermissionItem in fcPermissionItems)
            {
                var firmPermission = new FirmPermission();
                var fcaPermissionName = (fcaPermissionItem as JProperty)?.Name;
                firmPermission.PermissionName = fcaPermissionName;

                var baseFirmPermission = baseFirmPermissions.FirstOrDefault(bp =>
                                             bp.PermissionName != null && bp.PermissionName.Equals(fcaPermissionName,
                                                 StringComparison.InvariantCultureIgnoreCase)) ??
                                         baseFirmPermissions.FirstOrDefault(bp => bp.IsForOthers);

                if (baseFirmPermission != null)
                {
                    firmPermission.CategoryName = baseFirmPermission.CategoryName;
                }

                var permissionToken = fcaPermissionItem.Children().FirstOrDefault()?.Values<JToken>();
                if (permissionToken != null)
                {
                    foreach (var permissionContents in permissionToken)
                    {
                        if (permissionContents == null)
                        {
                            continue;
                        }

                        foreach (var permissionContent in permissionContents)
                        {
                            var permissionContentName = (permissionContent as JProperty)?.Name;
                            switch (permissionContentName)
                            {
                                case "Customer Type":
                                    firmPermission.CustomerTypes =
                                        permissionContent.Children().Values<string?>().ToList();
                                    break;
                                case "Investment Type":
                                    firmPermission.InvestmentTypes =
                                        permissionContent.Children().Values<string?>().ToList();
                                    break;
                                case "Limitation":
                                    firmPermission.Limitations =
                                        permissionContent.Children().Values<string?>().ToList();
                                    break;
                            }
                        }
                    }
                }

                firmPermissions.Add(firmPermission);
            }

            return firmPermissions;
        }

        public async Task<IEnumerable<Company>> SearchMatchedFirmsAsync(string companyName, bool isFirm,
            string companyNo = "", string companyAddress = "")
        {
            // Remove post code part
            var openParenthesis = companyName.LastIndexOf("(", StringComparison.Ordinal);
            var postcode = "";

            if (openParenthesis > 0)
            {
                var closeParenthesis = companyName.LastIndexOf(")", StringComparison.Ordinal);
                var length = closeParenthesis - openParenthesis;
                //extracted. Postcode: ME15 6YE
                postcode = companyName.Substring(openParenthesis + 1, length - 1);

                //if the company name contains like this 'Region: D-10785'
                //meaning this company isn't in UK. this is other country so we return null
                if (!string.IsNullOrEmpty(postcode) && postcode.ToLower().Contains("region:"))
                {
                    return new List<Company>();
                }

                // exclude open and close parenthesis
                companyName = companyName.Replace($" ({postcode})", string.Empty);
                companyName = RemoveLimitedWord(companyName);
            }

            var keyword = companyName.Contains("&") ? Uri.EscapeDataString(companyName) : companyName;
            var endpoint = $"{_baseUrl}/services/V0.1/Search?q={keyword}";
            var fcaCompanies = new List<FcaCompanyK>();

            if (_memoryCache.Get(endpoint) is List<FcaCompanyK> cachedValue)
            {
                fcaCompanies = cachedValue;
            }
            else
            {
                var taskListFcaResult = new List<Task<FcaCompanySearchResult>>();
                var fcaResultByCompanyNameFromFirstTask = await GetRemoteAsync(endpoint,
                    async (response) => await HandleFailureAsync<FcaCompanySearchResult>(endpoint, response));

                if (fcaResultByCompanyNameFromFirstTask is { ResultInfo: not null })
                {
                    var totalCount = !string.IsNullOrEmpty(fcaResultByCompanyNameFromFirstTask.ResultInfo.TotalCount)
                        ? int.Parse(fcaResultByCompanyNameFromFirstTask.ResultInfo.TotalCount)
                        : 0;
                    var perPage = !string.IsNullOrEmpty(fcaResultByCompanyNameFromFirstTask.ResultInfo.PerPage)
                        ? int.Parse(fcaResultByCompanyNameFromFirstTask.ResultInfo.PerPage)
                        : 1;
                    var noOfPages = CalculateNoOfPages(totalCount, perPage);

                    if (fcaResultByCompanyNameFromFirstTask.ResultInfo?.Next != null && noOfPages > 1)
                    {
                        var pagedUrls =
                            ConstructPagedUrls(fcaResultByCompanyNameFromFirstTask.ResultInfo.Next, noOfPages);

                        taskListFcaResult.AddRange(pagedUrls.Select(endPoint => GetRemoteAsync(endPoint,
                            async (response) => await HandleFailureAsync<FcaCompanySearchResult>(endPoint, response))));

                        if (taskListFcaResult.Any())
                        {
                            var results = await Task.WhenAll(taskListFcaResult);

                            if (results.Any())
                            {
                                foreach (var result in results)
                                {
                                    if (result?.FcaCompanies == null)
                                    {
                                        continue;
                                    }

                                    fcaCompanies.AddRange(result.FcaCompanies.ToList());
                                }
                            }
                        }
                    }

                    if (fcaResultByCompanyNameFromFirstTask.FcaCompanies != null)
                    {
                        fcaCompanies.AddRange(fcaResultByCompanyNameFromFirstTask.FcaCompanies.ToList());
                    }
                }
            }

            _memoryCache.AddOrGetExisting(endpoint, fcaCompanies,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));

            if (!fcaCompanies.Any())
            {
                return new List<Company>();
            }

            List<FcaCompanyK>? fcaCompaniesMatchByName = null;

            if (!string.IsNullOrEmpty(postcode))
            {
                fcaCompaniesMatchByName = SetCompanyAddressIfApplicable(
                    fcaCompanies.Where(fcac => fcac.Name.Contains(postcode)).ToList(),
                    companyAddress, postcode);
            }

            //just make sure the results still contains the companyname value
            if (fcaCompaniesMatchByName != null && fcaCompaniesMatchByName.Any())
            {
                fcaCompaniesMatchByName = EnsureNameStillMatch(fcaCompaniesMatchByName, companyName);
            }

            //some companies shows different postcode in company house vs fca
            //if this happens, we give another try ignoring post code checking in their company name
            if (!(fcaCompaniesMatchByName != null && fcaCompaniesMatchByName.Any()))
            {
                fcaCompaniesMatchByName =
                    SetCompanyAddressIfApplicable(fcaCompanies.ToList(), companyAddress);
            }

            if (isFirm)
            {
                var fcaResultByCompanyNumber =
                    await SearchFirmWithDetailsAsync(fcaCompaniesMatchByName, companyNo, companyAddress);
                var matchedByCompanyNumberResults = fcaResultByCompanyNumber.ToList();

                //just make sure the results still contains the companyname value
                if (matchedByCompanyNumberResults.Any())
                {
                    matchedByCompanyNumberResults = EnsureNameStillMatch(matchedByCompanyNumberResults, companyName);
                }

                if (matchedByCompanyNumberResults.Any())
                {
                    return matchedByCompanyNumberResults;
                }

                var fallbackResults =
                    _serviceMapper.Instance.Map<IEnumerable<Company>>(fcaCompaniesMatchByName).ToList();
                var listFallBack = EnsureNameStillMatch(fallbackResults, companyName);

                foreach (var itemFallBack in listFallBack.Where(itemFallBack =>
                             itemFallBack.CompanyNumber == AppConstants.NotApplicable))
                {
                    itemFallBack.CompanyNumber = companyNo;
                }

                return listFallBack;
            }

            var companies = _serviceMapper.Instance.Map<IEnumerable<Company>>(fcaCompaniesMatchByName);
            return companies.Where(x => x.Type.ToLower() == "firm");
        }

        public async Task<IEnumerable<Company>> SearchFirmsByFirmNameKeywordAsync(string keyword)
        {
            keyword = Uri.EscapeDataString(keyword);
            var endpoint = $"{_baseUrl}/services/V0.1/Search?q={keyword}";
            List<FcaCompanyK>? fcaCompanies;

            if (_memoryCache.Get(endpoint) is List<FcaCompanyK> cachedValue)
            {
                fcaCompanies = cachedValue;
            }
            else
            {
                var fcaResultByCompanyName = await GetRemoteAsync(endpoint,
                    async (response) => await HandleFailureAsync<FcaCompanySearchResult>(endpoint, response));

                if (fcaResultByCompanyName.FcaCompanies == null)
                {
                    return new List<Company>();
                }

                fcaCompanies = fcaResultByCompanyName.FcaCompanies.ToList();
            }

            _memoryCache.AddOrGetExisting(endpoint, fcaCompanies,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));

            var companies = _serviceMapper.Instance.Map<IEnumerable<Company>>(fcaCompanies);
            return companies;
        }

        public async Task<string> SearchFirmClientMoneyPermissionAsync(string firmRefNo)
        {
            var currentFirm = await SearchFcaFirmDetailsAsync(firmRefNo);
            var clientMoneyPermission = currentFirm.ClientMoneyPermission ?? "";
            return clientMoneyPermission;
        }

        public async Task<FcaFirmDetail> SearchFcaFirmDetailsAsync(string firmRefNo)
        {
            var endpoint = $"{_baseUrl}/services/V0.1/Firm/{firmRefNo}";
            if (_memoryCache.Get(endpoint) is FcaFirmDetail cachedValue)
            {
                return cachedValue;
            }

            var firmResult = await GetRemoteAsync(endpoint,
                async response => await HandleFailureAsync<FcaCompanyDetailsResult>(endpoint, response));
            var fcaFirmDetail = firmResult.Detail?.FirstOrDefault() ?? new FcaFirmDetail();

            _memoryCache.AddOrGetExisting(endpoint, fcaFirmDetail,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return fcaFirmDetail;
        }

        public async Task<IEnumerable<string?>> GetTradingNamesAsync(string firmRefNo)
        {
            var responseData = new List<Data>();
            var endpoint = $"{_baseUrl}/services/V0.1/Firm/{firmRefNo}/Names";

            do
            {
                var firmNamesResult = await GetRemoteAsync(endpoint,
                    async response => await HandleFailureAsync<FcaCompanyNamesResult>(endpoint, response));
                var data = firmNamesResult.Data?.FirstOrDefault(d => d.CurrentNames.Any());
                if (data != null)
                {
                    responseData.Add(data);
                }

                endpoint = firmNamesResult.ResultInfo?.Next;
            } while (!string.IsNullOrEmpty(endpoint));

            var tradingNames = responseData.SelectMany(rd => rd.CurrentNames
                .Where(d => d.Status is "Trading" or "Registered")
                .Select(s => s.Name));
            return tradingNames;
        }

        public async Task<IEnumerable<FcaAddressDetails?>> GetFirmAddressDetailsAsync(string firmRefNo,
            string addressType)
        {
            var endpoint = $"{_baseUrl}/services/V0.1/Firm/{firmRefNo}/Address";

            if (!string.IsNullOrEmpty(addressType))
            {
                endpoint += $"?Type={addressType}";
            }

            var firmAddressResult = await GetRemoteAsync(endpoint,
                async (response) => await HandleFailureAsync<FcaCompanyAddressesResult>(endpoint, response));

            return firmAddressResult.Addresses ?? Enumerable.Empty<FcaAddressDetails>();
        }

        public async Task<Company?> GetMatchedCompanyAsync(string companyName, string companyNumber)
        {
            if (!_isGetFirmRefNoForDirectorship)
            {
                return null;
            }

            var fcaCompanies = await SearchCompanyByNameWithCachingAsync(companyName);

            if (fcaCompanies == null)
            {
                return null;
            }

            var fcaCompaniesList = fcaCompanies.ToList();

            if (!fcaCompaniesList.Any())
            {
                return null;
            }

            Company? found = null;

            // to make sure check if same company number
            var companyDetailsTaskList = new List<Task<FcaCompanyDetailsResult?>>();
            var companyDetailsDictionary = new Dictionary<string, FcaCompanyK>();

            foreach (var item in fcaCompaniesList.Where(item =>
                         !string.IsNullOrEmpty(item.Url) && !string.IsNullOrEmpty(item.ReferenceNo)))
            {
                companyDetailsTaskList.Add(GetFirmDetailsWithCachingAsync(item));
                companyDetailsDictionary.Add(item.ReferenceNo, item);
            }

            var companyDetailsTaskResults = new List<FcaCompanyDetailsResult?>();
            var result = await Task.WhenAll(companyDetailsTaskList);
            companyDetailsTaskResults = result?.ToList() ?? new List<FcaCompanyDetailsResult?>();

            foreach (var item in companyDetailsTaskResults)
            {
                if (item?.Detail == null)
                {
                    continue;
                }

                var foundFcaFirm = item.Detail.FirstOrDefault(d => d.CompaniesHouseNumber == companyNumber);
                var key = foundFcaFirm?.FirmReferenceNumber;

                if (foundFcaFirm == null || string.IsNullOrEmpty(key) || !companyDetailsDictionary.ContainsKey(key))
                {
                    continue;
                }

                var foundItem = companyDetailsDictionary[key];

                if (foundItem == null)
                {
                    continue;
                }

                found = _serviceMapper.Instance.Map<Company>(foundItem);
                found.CompanyNumber = foundFcaFirm.CompaniesHouseNumber ?? AppConstants.NotApplicable;
                break;
            }

            return found;
        }

        public async Task<IEnumerable<FcaIndividual>> GetFirmIndividualsAsync(string firmRefNo)
        {
            var endpoint = $"{_baseUrl}/services/V0.1/Firm/{firmRefNo}/Individuals";
            if (_memoryCache.Get(endpoint) is List<FcaIndividual> cachedValue)
            {
                return cachedValue;
            }

            var fcaIndividuals = new List<FcaIndividual>();

            var firstPageIndividualsResult = await GetRemoteAsync(endpoint,
                async response => await HandleFailureAsync<FcaIndividualsResult>(endpoint, response));

            //TEMP: This triggers rate-limiting on CF retrieval, disable now for demo
            //var allPagesIndividualsResults = await GetAllPagesResults(firstPageIndividualsResult, firstPageIndividualsResult.ResultInfo);
            var allPagesIndividualsResults = new List<FcaIndividualsResult> { firstPageIndividualsResult };

            foreach (var individualResult in allPagesIndividualsResults)
            {
                if (individualResult.Data != null)
                {
                    fcaIndividuals.AddRange(individualResult.Data);
                }
            }

            var individuals = fcaIndividuals.DistinctBy(x => x.Irn).ToList();
            await SetCurrentPreviousRoles(individuals, firmRefNo);

            _memoryCache.AddOrGetExisting(endpoint, individuals,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));

            /*// Check if match with FCA website (https://register.fca.org.uk/s/firm?id=001b000003d5ixrAAA)
            var currentIndividuals = individuals.Count(i => i.CurrentRoles.Any());
            var previousIndividuals = individuals.Count(i => i.PreviousRoles.Any());*/

            return individuals;
        }

        private async Task SetCurrentPreviousRoles(List<FcaIndividual> fcaIndividuals, string firmRefNo)
        {
            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = 10
            };
            await Parallel.ForEachAsync(fcaIndividuals, parallelOptions, async (individual, _) =>
            {
                var individualCf = await GetIndividualControlledFunctionsAsync(individual.Irn ?? "");
                individual.CurrentRoles = individualCf.RoleNames
                    .Where(r => r is { IsCurrent: true, Url: not null } && r.Url.Contains(firmRefNo)).ToList();
                individual.PreviousRoles = individualCf.RoleNames
                    .Where(r => r is { IsCurrent: false, Url: not null } && r.Url.Contains(firmRefNo)).ToList();
            });
        }

        public async Task<IEnumerable<FcaAppointedRepresentative>> GetAppointedRepresentativesAsync(
            string firmRefNo, string type)
        {
            var arCurrentResults = new List<FcaAppointedRepresentative>();
            var arPreviousResults = new List<FcaAppointedRepresentative>();

            var baseEndpoint = $"{_baseUrl}/services/V0.1/Firm/{firmRefNo}/AR";
            if (_memoryCache.Get(baseEndpoint) is List<FcaAppointedRepresentative> cachedValue)
            {
                arCurrentResults = cachedValue;
            }
            else
            {
                var firstPageArResult = await GetRemoteAsync(baseEndpoint,
                    async response =>
                        await HandleFailureAsync<FcaAppointedRepresentativesResult>(baseEndpoint, response));
                var allPagesArResults = await GetAllPagesResults(firstPageArResult, firstPageArResult.ResultInfo);

                foreach (var arResult in allPagesArResults)
                {
                    var currentPagedResults = arResult?.Data?.CurrentAppointedRepresentatives;
                    if (currentPagedResults != null && currentPagedResults.Any())
                    {
                        arCurrentResults.AddRange(currentPagedResults);
                    }

                    var previousPagedResults = arResult?.Data?.PreviousAppointedRepresentatives;
                    if (previousPagedResults != null && previousPagedResults.Any())
                    {
                        arPreviousResults.AddRange(previousPagedResults);
                    }
                }

                foreach (var arPreviousResult in arPreviousResults)
                {
                    arPreviousResult.IsCurrentRepresentative = false;
                    arCurrentResults.Add(arPreviousResult);
                }

                _memoryCache.AddOrGetExisting(baseEndpoint, arCurrentResults,
                    DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            }

            var filteredResults = arCurrentResults
                .Where(f => f.RecordSubType?.ToLower() == type.ToLower())
                .ToList();

            // TODO. need to get other details as below:
            // Principal Firm Details
            //  - Addresses, Trading Names, Contact Number, Website
            return filteredResults;
        }

        public async Task<IEnumerable<FcaRegulator>> GetFirmRegulatorsAsync(string firmRefNo)
        {
            var endpoint = $"{_baseUrl}/services/V0.1/Firm/{firmRefNo}/Regulators";

            if (_memoryCache.Get(endpoint) is List<FcaRegulator> cachedValue)
            {
                return cachedValue;
            }

            var regulatorsResult = await GetRemoteAsync(endpoint,
                async response => await HandleFailureAsync<FcaRegulatorsResult>(endpoint, response));
            var regulators = regulatorsResult?.Data ?? new List<FcaRegulator>();
            _memoryCache.AddOrGetExisting(endpoint, regulators,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return regulators;
        }

        public async Task<FcaRolesResult> GetIndividualControlledFunctionsAsync(string individualRefNo)
        {
            var endpoint = $"{_baseUrl}/services/V0.1/Individuals/{individualRefNo}/CF";
            if (_memoryCache.Get(endpoint) is FcaRolesResult cachedValue)
            {
                return cachedValue;
            }

            var cfResult = await GetRemoteAsync(endpoint,
                async response => await HandleFailureAsync<FcaIndividualControlledFunctionsResult>(endpoint, response));
            if (cfResult.Data == null || !cfResult.Data.Any())
            {
                return new FcaRolesResult();
            }

            // FCA API returns Data in an array[] of Current{} and Previous{}.
            var rolesData = cfResult.Data[0];
            var currentRoleItems = (rolesData.CurrentControlledFunctions as JObject)!.AsJEnumerable();
            var previousRoleItems = (rolesData.PreviousControlledFunctions as JObject)!.AsJEnumerable();

            var fcaIndividualRoles = new List<FcaIndividualRole>();

            if (currentRoleItems != null && currentRoleItems.Any())
            {
                AddRoleItems(currentRoleItems, true);
            }

            if (previousRoleItems != null && previousRoleItems.Any())
            {
                AddRoleItems(previousRoleItems);
            }

            var fcaRolesResult = new FcaRolesResult
            {
                RoleNames = fcaIndividualRoles
            };

            _memoryCache.AddOrGetExisting(endpoint, fcaRolesResult,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return fcaRolesResult;

            void AddRoleItems(IEnumerable<JToken> list, bool isCurrent = false)
            {
                fcaIndividualRoles.AddRange(from item in list
                    let roleToken = item.Children().FirstOrDefault()
                    where roleToken != null
                    select new FcaIndividualRole
                    {
                        Id = (item as JProperty)?.Name,
                        CustomerEngagementMethod = roleToken.Value<string?>("Customer Engagement Method") ?? "",
                        EndDate = roleToken.Value<string?>("End Date") ?? "",
                        Restriction = roleToken.Value<string?>("Restriction") ?? "",
                        EffectiveDate = roleToken.Value<string?>("Effective Date") ?? "",
                        FirmName = roleToken.Value<string?>("Firm Name") ?? "",
                        Name = roleToken.Value<string?>("Name") ?? "",
                        Url = roleToken.Value<string?>("URL") ?? "",
                        IsCurrent = isCurrent
                    });
            }
        }

        protected override HttpRequestMessage CreateRequestMessageGet(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            var email = Environment.GetEnvironmentVariable("FcaUserName", EnvironmentVariableTarget.Process);
            var apiKey = Environment.GetEnvironmentVariable("FcaApiKey", EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException(
                    $"Email and api key should not be null in {nameof(FcaService)}.{nameof(CreateRequestMessageGet)}");
            }

            request.Headers.Add("X-Auth-Email", email);
            request.Headers.Add("X-Auth-Key", apiKey);
            return request;
        }

        protected override HttpRequestMessage CreateRequestMessagePost(string endpoint, HttpContent httpContent)
        {
            throw new NotImplementedException();
        }

        #region Private Methods

        private static IEnumerable<string> ConstructPagedUrls(string endpoint, int noOfPages)
        {
            // ex. https://register.fca.org.uk/services/V0.1/Search?q=THE RIGHT MORTGAGE&pgnp=2
            var pagedUrls = new List<string>
            {
                endpoint
            };

            var uriBuilder = new UriBuilder(endpoint);
            var queryString = uriBuilder.Query.TrimStart('?');
            var queryParams = HttpUtility.ParseQueryString(queryString);

            // Replace the 'pgnp' parameter with incremental values
            for (var i = 3; i < noOfPages + 1; i++)
            {
                queryParams["pgnp"] = i.ToString();
                uriBuilder.Query = queryParams.ToString();
                var modifiedUrl = uriBuilder.ToString();
                pagedUrls.Add(modifiedUrl);
            }

            return pagedUrls;
        }

        private static int CalculateNoOfPages(int totalRecords, int recordsPerPage)
        {
            if (totalRecords <= 0 || recordsPerPage <= 0)
            {
                return 0;
            }

            return (int)Math.Ceiling((double)totalRecords / recordsPerPage);
        }

        private async Task<IEnumerable<Company>> SearchFirmWithDetailsAsync(
            IReadOnlyCollection<FcaCompanyK> fcaCompanies,
            string companyNo, string companyAddress)
        {
            if (!_isFcaSearchDetailEnabled)
            {
                var companies = _serviceMapper.Instance.Map<IEnumerable<Company>>(fcaCompanies);
                return companies;
            }

            var listOfCompanies = new List<Company>();

            foreach (var fcaCompany in fcaCompanies.Where(fcaCompany => !string.IsNullOrEmpty(fcaCompany.Url)))
            {
                FcaCompanyDetailsResult? firmResult;
                var appointedRepresentatives = new List<FcaAppointedRepresentative>();
                FcaFirmDetail? fcaFirmDetail = null;

                if (!string.IsNullOrEmpty(fcaCompany.ReferenceNo))
                {
                    var arEndpoint = $"{_baseUrl}/services/V0.1/Firm/{fcaCompany.ReferenceNo}/AR";
                    var isBothApiRequestsCached = true;

                    if (_memoryCache.Get(arEndpoint) is List<FcaAppointedRepresentative> representativesCache)
                    {
                        appointedRepresentatives = representativesCache;
                    }
                    else
                    {
                        isBothApiRequestsCached = false;
                    }

                    if (_memoryCache.Get(fcaCompany.Url) is FcaFirmDetail firmDetailCache)
                    {
                        fcaFirmDetail = firmDetailCache;
                    }
                    else
                    {
                        isBothApiRequestsCached = false;
                    }

                    if (!isBothApiRequestsCached)
                    {
                        var arResultTask = GetRemoteAsync(arEndpoint,
                            async (response) =>
                                await HandleFailureAsync<FcaAppointedRepresentativesResult>(arEndpoint, response));
                        var firmResultTask = GetRemoteAsync(fcaCompany.Url,
                            async (response) =>
                                await HandleFailureAsync<FcaCompanyDetailsResult>(fcaCompany.Url, response));

                        await Task.WhenAll(firmResultTask, arResultTask);

                        var appointedRepresentativesResult = await arResultTask;
                        firmResult = await firmResultTask;
                        fcaFirmDetail = firmResult?.Detail?.FirstOrDefault() ?? new FcaFirmDetail();

                        var arResults = appointedRepresentativesResult?.Data?.CurrentAppointedRepresentatives ??
                                        new List<FcaAppointedRepresentative>();
                        _memoryCache.AddOrGetExisting(arEndpoint, arResults,
                            DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneHourInMinutes));
                        _memoryCache.AddOrGetExisting(fcaCompany.Url, fcaFirmDetail,
                            DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneHourInMinutes));
                        appointedRepresentatives = arResults;
                    }
                }
                else
                {
                    firmResult = await GetRemoteAsync(fcaCompany.Url,
                        async (response) =>
                            await HandleFailureAsync<FcaCompanyDetailsResult>(fcaCompany.Url, response));
                    fcaFirmDetail = firmResult?.Detail?.FirstOrDefault();
                }

                var firm = fcaFirmDetail;
                var companyNumber = firm?.CompaniesHouseNumber;

                if (companyNumber != companyNo)
                {
                    continue;
                }

                var mappedCompany = _serviceMapper.Instance.Map<Company>(fcaCompany);
                mappedCompany.CompanyNumber = companyNo ?? "";
                mappedCompany.Address = companyAddress ?? "";
                mappedCompany.AppointedRepresentatives = appointedRepresentatives;

                listOfCompanies.Add(mappedCompany);
            }

            return listOfCompanies;
        }

        private static List<FcaCompanyK> SetCompanyAddressIfApplicable(List<FcaCompanyK> fcaCompaniesMatchByName,
            string companyAddress, string postCode = "")
        {
            if (string.IsNullOrEmpty(postCode))
            {
                // if no postcode, set all addresses to empty.
                fcaCompaniesMatchByName.ForEach(c => c.CompanyAddress = string.Empty);
                return fcaCompaniesMatchByName;
            }

            foreach (var fcaCompany in from fcaCompany in fcaCompaniesMatchByName
                     let isSamePostcode = fcaCompany.Name.Contains(postCode)
                     where isSamePostcode
                     select fcaCompany)
            {
                fcaCompany.CompanyAddress = companyAddress;
            }

            return fcaCompaniesMatchByName;
        }

        private static string RemoveLimitedWord(string companyName)
        {
            if (string.IsNullOrEmpty(companyName))
            {
                return "";
            }

            var idx = companyName.ToLower().IndexOf("limited", StringComparison.Ordinal);
            string cleanedName;
            if (idx > 0)
            {
                cleanedName = companyName[..idx].Trim();
                return cleanedName;
            }

            var idx2 = companyName.ToLower().IndexOf("ltd", StringComparison.Ordinal);
            if (idx2 > 0)
            {
                cleanedName = companyName[..idx2].Trim();
                return cleanedName;
            }

            cleanedName = companyName;
            return cleanedName;
        }

        private static List<FcaCompanyK> EnsureNameStillMatch(IReadOnlyCollection<FcaCompanyK> fcaCompanies,
            string companyName)
        {
            if (string.IsNullOrEmpty(companyName) || !fcaCompanies.Any())
            {
                return fcaCompanies.ToList();
            }

            var results = fcaCompanies.Where(x => x.Name != null && x.Name.ToLower().Contains(companyName.ToLower()))
                .ToList();

            if (results.Any())
            {
                return results;
            }

            if (!companyName.ToLower().Contains(" and "))
            {
                return results;
            }

            var variedName = companyName.Replace(" AND ", " & ").Replace(" and ", " & ");
            results = fcaCompanies.Where(x => x.Name != null && x.Name.ToLower().Contains(variedName.ToLower()))
                .ToList();
            return results;
        }

        private static List<Company> EnsureNameStillMatch(IReadOnlyCollection<Company> companies, string companyName)
        {
            if (string.IsNullOrEmpty(companyName) || !companies.Any())
            {
                return companies.ToList();
            }

            var results = companies.Where(x => x.CompanyName.ToLower().Contains(companyName.ToLower())).ToList();

            if (results.Any())
            {
                return results;
            }

            if (!companyName.ToLower().Contains(" and "))
            {
                return results;
            }

            var variedName = companyName.Replace(" AND ", " & ").Replace(" and ", " & ");
            results = companies.Where(x => x.CompanyName.ToLower().Contains(variedName.ToLower())).ToList();
            return results;
        }

        private async Task<IEnumerable<FcaCompanyK>> SearchCompanyByNameWithCachingAsync(string companyName)
        {
            companyName = companyName.Replace("&", "and");
            var endpoint = $"{_baseUrl}/services/V0.1/Search?q={companyName}";

            if (_memoryCache.Get(endpoint) is List<FcaCompanyK> cachedValue)
            {
                return cachedValue;
            }

            var fcaResultByCompanyName = await GetRemoteAsync(endpoint,
                async (response) => await HandleFailureAsync<FcaCompanySearchResult>(endpoint, response));
            var fcaCompanies = fcaResultByCompanyName.FcaCompanies ?? new List<FcaCompanyK>();
            _memoryCache.AddOrGetExisting(endpoint, fcaCompanies,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return fcaCompanies;
        }

        private async Task<FcaCompanyDetailsResult?> GetFirmDetailsWithCachingAsync(FcaCompanyK fcaCompany)
        {
            if (string.IsNullOrEmpty(fcaCompany.Url))
            {
                return new FcaCompanyDetailsResult();
            }

            if (_memoryCache.Get(fcaCompany.Url) is FcaCompanyDetailsResult cachedValue)
            {
                return cachedValue;
            }

            var result = await GetRemoteAsync(fcaCompany.Url,
                async (response) => await HandleFailureAsync<FcaCompanyDetailsResult>(fcaCompany.Url, response));
            result ??= new FcaCompanyDetailsResult();
            _memoryCache.AddOrGetExisting(fcaCompany.Url, result,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return result;
        }

        private async Task<List<T>> GetAllPagesResults<T>(T firstArResult, ResultInfo? resultInfo) where T : new()
        {
            var allFcaResults = new List<T> { firstArResult };

            var nextEndpoint = resultInfo?.Next;
            if (nextEndpoint == null)
            {
                return allFcaResults;
            }

            // FCA now uses SalesForce return URL for succeeding pages which 
            // is invalid in FCA API call and throws error// 
            // Old: "https://register.fca.org.uk/services/V0.1/Firm/826479
            // New: "https://intact.my.salesforce.com/services/V0.1/Firm/826479

            nextEndpoint = nextEndpoint.Replace("intact.my.salesforce.com", "register.fca.org.uk");

            if (!int.TryParse(resultInfo?.TotalCount, out var totalCount))
            {
                totalCount = 0;
            }

            if (!int.TryParse(resultInfo?.PerPage, out var perPage))
            {
                perPage = 1;
            }

            var noOfPages = CalculateNoOfPages(totalCount, perPage);
            var nextPageEndpoints = ConstructPagedUrls(nextEndpoint, noOfPages);
            var arTasks = nextPageEndpoints.Select(endpoint =>
                GetRemoteAsync(endpoint, async response => await HandleFailureAsync<T>(endpoint, response))).ToList();

            const int batchSize = 10;
            var tasksBatches = arTasks.Chunk(batchSize);
            foreach (var tasksBatch in tasksBatches)
            {
                allFcaResults.AddRange(await Task.WhenAll(tasksBatch));
            }

            return allFcaResults;
        }

        #endregion
    }
}