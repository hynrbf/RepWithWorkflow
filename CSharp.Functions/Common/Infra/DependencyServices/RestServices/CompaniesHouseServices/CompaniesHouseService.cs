using Azure.Storage.Blobs.Models;
using Common.Entities;
using System.Runtime.Caching;

namespace Common.Infra
{
    public class CompaniesHouseService : RestServiceBase, ICompaniesHouseService
    {
        private const string BaseUrl = "https://api.company-information.service.gov.uk";
        private const string DocumentBaseUrl = "https://document-api.company-information.service.gov.uk";
        private const string IndividualControllerKind = "individual-person-with-significant-control";
        private const string CorporateControllerKind = "corporate-entity-person-with-significant-control";
        private const string Dissolved = "dissolved";
        private const int MaxRecurseLevel = 5;

        private readonly IServiceMapper _serviceMapper = new ServiceMapper();
        private readonly IFcaService _fcaService;
        private readonly MemoryCache _memoryCache;
        private readonly IBlobContainerService _blobContainerClientService;

        public CompaniesHouseService(IFcaService faService, IBlobContainerService blobContainerClientService)
        {
            _fcaService = faService;
            _blobContainerClientService = blobContainerClientService;
            _memoryCache = MemoryCache.Default;
        }

        public async Task<IEnumerable<Company>> SearchCompaniesWithDetailsAsync(string keyword, int itemsPerPage)
        {
            keyword = keyword.Replace("&", "and");
            var endpoint = $"{BaseUrl}/search/companies?q={keyword}&items_per_page={itemsPerPage}";

            if (_memoryCache.Get(endpoint) is IEnumerable<Company> cachedValue)
            {
                return cachedValue;
            }

            var response = await GetRemoteAsync(endpoint,
                async (response) => await HandleFailureAsync<CompaniesHouseResultK>(endpoint, response));
            var mappedItems = _serviceMapper.Instance.Map<IEnumerable<Company>>(response.Items);
            //remove non active companies
            //if you know other status that means not active, just add it here
            var output = mappedItems.Where(c =>
                !string.IsNullOrEmpty(c.CompanyStatus) && c.CompanyStatus.ToLower() != Dissolved);
            _memoryCache.AddOrGetExisting(endpoint, output,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return output;
        }

        public async Task<IEnumerable<CompanyOfficer>> GetCompanyOfficersAsync(string companyNumber)
        {
            var endpoint = $"{BaseUrl}/company/{companyNumber}/officers";

            if (_memoryCache.Get(endpoint) is List<CompanyOfficer> cachedValue)
            {
                return cachedValue;
            }

            var response = await GetRemoteAsync<CompanyOfficersResult>(endpoint);
            var itemList = response.Items.ToList();
            var tasksList = new List<Task<CompanyOfficerAppointmentDetails>>();
            var companyOfficerDictionary = new Dictionary<string, CompanyOfficer>();

            foreach (var item in itemList)
            {
                var appointmentEndpoint = item.Links?.Officer?.Appointments;

                if (string.IsNullOrEmpty(appointmentEndpoint))
                {
                    continue;
                }

                var officerEndpoint = $"{BaseUrl}{appointmentEndpoint}";

                if (string.IsNullOrEmpty(item.Name))
                {
                    continue;
                }

                //keyName is forename + surname because this is the result format in the api
                var keyName = $"{item.Forename}{item.Surname}".RemoveNonAlphaNumeric().ToLower();

                if (companyOfficerDictionary.ContainsKey(keyName))
                {
                    continue;
                }

                tasksList.Add(GetRemoteAsync(officerEndpoint,
                    async (companyOfficerResponse) =>
                        await HandleFailureAsync<CompanyOfficerAppointmentDetails>(officerEndpoint,
                            companyOfficerResponse)));
                companyOfficerDictionary.Add(keyName, item);
            }

            if (!tasksList.Any())
            {
                return companyOfficerDictionary.Values;
            }

            var results = await Task.WhenAll(tasksList);

            foreach (var result in results)
            {
                var firstItem = result.Appointments.FirstOrDefault();

                if (firstItem == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(firstItem.Name))
                {
                    continue;
                }

                var keyName = firstItem.Name.RemoveNonAlphaNumeric().ToLower();

                if (!companyOfficerDictionary.ContainsKey(keyName))
                {
                    continue;
                }

                companyOfficerDictionary[keyName].Title = firstItem.NameElements?.Title;
            }

            var finalOutput = companyOfficerDictionary.Values.ToList();
            _memoryCache.AddOrGetExisting(endpoint, finalOutput,
                DateTimeOffset.Now.AddDays(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return finalOutput;
        }

        public async Task<IEnumerable<CompanyOfficer>> GetCompanyActiveDirectorsAsync(string companyNumber)
        {
            var officers = await GetCompanyOfficersAsync(companyNumber);
            var activeDirectors =
                officers.Where(o =>
                    "director".Equals(o.OfficerRole,
                        StringComparison.OrdinalIgnoreCase) && // Get only if role is "director"
                    string.IsNullOrEmpty(o.ResignedOn)); // Get only director who is not resigned yet
            return activeDirectors;
        }

        //This is slow operation but is already speedy now
        //Needs to revisit if results are still really good.
        public async Task<IEnumerable<CompanyOfficerAppointmentDetails>> GetDirectorsAndAppointmentsAsync(
            string companyNumber)
        {
            var activeDirectors = (await GetCompanyActiveDirectorsAsync(companyNumber)).ToList();
            var companyOfficerAppointmentDetails = new List<CompanyOfficerAppointmentDetails>();
            var directorAppointmentsTaskList = (from director in activeDirectors
                select director.Links?.Officer?.Appointments
                into appointmentsLink
                where !string.IsNullOrEmpty(appointmentsLink)
                select GetAppointmentsAsync(appointmentsLink));

            var appointmentsResult = await Task.WhenAll(directorAppointmentsTaskList);

            var companyProfileTaskList = new List<Task<CompanyProfileK?>>();
            var companyMatchedTaskList = new List<Task<Company?>>();

            foreach (var result in appointmentsResult)
            {
                if (result == null)
                {
                    continue;
                }

                foreach (var appointment in result.Appointments.Where(appointment =>
                             !string.IsNullOrEmpty(appointment.CompanyNumber) &&
                             !string.IsNullOrEmpty(appointment.CompanyName) &&
                             !appointment.CompanyStatus.Equals(Dissolved,
                                 StringComparison
                                     .OrdinalIgnoreCase))) // Get only appointments on "non-dissolved" company
                {
                    if (string.IsNullOrEmpty(appointment.CompanyNumber) ||
                        string.IsNullOrEmpty(appointment.CompanyName))
                    {
                        continue;
                    }

                    companyProfileTaskList.Add(GetCompanyProfileAsync(appointment.CompanyNumber));
                    companyMatchedTaskList.Add(
                        _fcaService.GetMatchedCompanyAsync(appointment.CompanyName, appointment.CompanyNumber));
                }
            }

            var companyProfileTaskResults = await Task.WhenAll(companyProfileTaskList);
            var companyMatchedResults = await Task.WhenAll(companyMatchedTaskList);

            companyMatchedResults.ToList().ForEach(c =>
            {
                if (!string.IsNullOrEmpty(c?.CompanyName))
                {
                    c.CompanyName = c.CompanyName.Trim().ToUpper();
                }
            });

            foreach (var result in appointmentsResult)
            {
                foreach (var appointment in result.Appointments)
                {
                    if (string.IsNullOrEmpty(appointment.CompanyNumber) ||
                        string.IsNullOrEmpty(appointment.CompanyName))
                    {
                        continue;
                    }

                    var companyProfile =
                        companyProfileTaskResults.FirstOrDefault(
                            cp => cp != null && cp.CompanyNumber == appointment.CompanyNumber);
                    appointment.NatureOfBusiness = companyProfile?.SicCodes.FirstOrDefault();
                    // Map Country code
                    appointment.CountryCode = GetCountryCodeFromCountryName(appointment.Country);

                    if (appointment.CompanyStatus.Equals(Dissolved, StringComparison.OrdinalIgnoreCase) &&
                        string.IsNullOrEmpty(appointment.ResignedOn))
                    {
                        //newly added, we didn't use task when all here. will be handling right amount of fast data gathering
                        //in other ticket soon. we cant call api very fast because of rate limiting
                        var dissolvedCompanyProfile = await GetCompanyProfileAsync(appointment.CompanyNumber);
                        appointment.ResignedOn = dissolvedCompanyProfile?.DateOfCessation;
                    }

                    appointment.OriginalResignedOn = appointment.ResignedOn;

                    var foundMatch = companyMatchedResults?.FirstOrDefault(cm =>
                        cm != null && cm.CompanyName == appointment.CompanyName);
                    appointment.FirmReferenceNumber =
                        foundMatch == null ? "Not available" : foundMatch.FirmReferenceNo;
                }

                var name = result.Appointments.FirstOrDefault()?.Name ?? string.Empty;
                result.FullName = Helpers.CapitalizeOnlyFirstLetterOfWord(name);

                foreach (var director in from director in activeDirectors
                         let nameSegment = director.Name?.ToLower().Split(',')
                         where nameSegment is { Length: >= 1 }
                         let fullName = $"{nameSegment[1].Trim()} {nameSegment[0].Trim()}"
                         where result.FullName.ToLower() == fullName && !string.IsNullOrEmpty(director.Nationality)
                         select director)
                {
                    if (director.Nationality == null)
                    {
                        continue;
                    }

                    if (result.Nationalities.FirstOrDefault(n =>
                            string.Equals(n, director.Nationality, StringComparison.OrdinalIgnoreCase)) == null)
                    {
                        result.Nationalities.Add(director.Nationality);
                    }
                }

                companyOfficerAppointmentDetails.Add(result);
            }

            return companyOfficerAppointmentDetails;
        }

        //This is slow operation but is already speedy now
        //Needs to revisit if results are still really good.
        public async Task<IEnumerable<Controller>> GetControllingInterestAsync(string companyNumber)
        {
            /*
                1. Get Person With Significant Control (idv)
                2. Get Company Directors
                3. Look for appointments of (1) From (2)
                    IF has any
                        Do code
                    Else
                        Blank directorships and controlling interest
             */

            var controllingInterests = new List<Controller>();
            var personWithSignificantControlResponse = await GetPersonsWithSignificantControlAsync(companyNumber);
            var personsWithSignificantControl = personWithSignificantControlResponse?.Controllers?
                .Where(c => c.Kind == IndividualControllerKind)
                .ToList() ?? new List<Controller>();

            var companyDirectors = (await GetCompanyActiveDirectorsAsync(companyNumber)).ToList();
            var appointmentTasksList = new List<Task<CompanyOfficerAppointmentDetails?>>();
            var directorDictionary = new Dictionary<string, CompanyOfficer>();

            foreach (var person in personsWithSignificantControl)
            {
                var nameSearchKey = $"{person.NameElements?.Surname}, {person.NameElements?.Forename}";

                if (string.IsNullOrEmpty(nameSearchKey))
                {
                    continue;
                }

                var foundAsDirector = companyDirectors
                    .FirstOrDefault(d =>
                        !string.IsNullOrEmpty(d?.Name) &&
                        d.Name.Contains(nameSearchKey, StringComparison.OrdinalIgnoreCase));

                if (foundAsDirector == null)
                {
                    var companyProfile = await GetCompanyProfileAsync(companyNumber);
                    person.Directorships = new CompanyOfficerAppointmentDetails();
                    person.ControllingInterests = new List<CompanyControllingInterestDetails>
                    {
                        new()
                        {
                            CompanyName = companyProfile?.CompanyName,
                            CompanyNumber = companyProfile?.CompanyNumber ?? companyNumber,
                            NatureOfBusiness = companyProfile?.SicCodes.FirstOrDefault(),
                            CountryOfIncorporation = person.Address?.Country,
                            CountryCode = GetCountryCodeFromCountryName(person.Address?.Country),
                            FirmReferenceNumber = person.FirmReferenceNumber,
                            PercentageOfCapital = person.PercentageOfCapital,
                            PercentageOfVotingRights = person.PercentageOfVotingRights
                        }
                    };
                    person.CompanyName = companyProfile?.CompanyName;
                    person.CompanyNumber = companyProfile?.CompanyNumber ?? companyNumber;
                    person.NatureOfBusiness = companyProfile?.SicCodes.FirstOrDefault();
                    controllingInterests.Add(person);
                    continue;
                }

                var appointmentEndpoint = $"{foundAsDirector.Links?.Officer?.Appointments}";

                if (string.IsNullOrEmpty(appointmentEndpoint))
                {
                    continue;
                }

                appointmentTasksList.Add(GetAppointmentsAsync(appointmentEndpoint));
                var key = $"{foundAsDirector.Forename.ToLower()} {foundAsDirector.Surname.ToLower()}";
                directorDictionary.TryAdd(key, foundAsDirector);
            }

            // Getting appointments if director
            var appointmentTasksResults = await Task.WhenAll(appointmentTasksList);
            var controllingInterestTaskList = new List<Task<ControllersResult?>>();
            var companyProfileTaskList = new List<Task<CompanyProfileK?>>();
            var matchedCompanyTaskList = new List<Task<Company?>>();
            var activeAppointmentDictionary = new Dictionary<string, CompanyOfficeAppointment>();

            foreach (var foundAppointment in appointmentTasksResults)
            {
                if (foundAppointment == null || !foundAppointment.Appointments.Any())
                {
                    continue;
                }

                var firstItem = foundAppointment.Appointments.FirstOrDefault();

                if (firstItem == null)
                {
                    continue;
                }

                var fullName = firstItem.Name?.ToLower();

                if (string.IsNullOrEmpty(fullName) || !directorDictionary.ContainsKey(fullName))
                {
                    continue;
                }

                var director = directorDictionary[fullName];

                var appointments = foundAppointment.Appointments.Where(a =>
                    a.Name?.ToLower() == $"{director.Forename.ToLower()} {director.Surname.ToLower()}" &&
                    string.IsNullOrEmpty(a.ResignedOn));

                var activeAppointmentList = appointments.ToList();

                if (!activeAppointmentList.Any())
                {
                    continue;
                }

                foreach (var activeAppointment in activeAppointmentList)
                {
                    if (activeAppointment.AppointedTo == null ||
                        activeAppointment.AppointedTo.CompanyStatus?.ToLower() != "active")
                    {
                        continue;
                    }

                    var activeAppointmentCompanyNumber =
                        activeAppointment.AppointedTo?.CompanyNumber ?? activeAppointment.CompanyNumber;
                    var key = $"{fullName}_{activeAppointmentCompanyNumber}"; // e.g. ["tania joan stretton_09161378"]

                    if (string.IsNullOrEmpty(activeAppointmentCompanyNumber) ||
                        activeAppointmentDictionary.ContainsKey(key))
                    {
                        continue;
                    }

                    controllingInterestTaskList.Add(
                        GetPersonsWithSignificantControlAsync(activeAppointmentCompanyNumber));
                    companyProfileTaskList.Add(GetCompanyProfileAsync(activeAppointmentCompanyNumber));

                    if (string.IsNullOrEmpty(key) || activeAppointmentDictionary.ContainsKey(key))
                    {
                        continue;
                    }

                    activeAppointmentDictionary.Add(key, activeAppointment);

                    if (string.IsNullOrEmpty(activeAppointment.CompanyName) ||
                        string.IsNullOrEmpty(activeAppointment.CompanyNumber))
                    {
                        continue;
                    }

                    matchedCompanyTaskList.Add(
                        _fcaService.GetMatchedCompanyAsync(activeAppointment.CompanyName,
                            activeAppointment.CompanyNumber));
                }
            }

            var controllingInterestTaskListResults = await Task.WhenAll(controllingInterestTaskList);
            var companyProfileTaskListResults = await Task.WhenAll(companyProfileTaskList);
            var matchedCompanyTaskListResults = await Task.WhenAll(matchedCompanyTaskList);

            foreach (var controllingInterestResult in controllingInterestTaskListResults)
            {
                if (string.IsNullOrEmpty(controllingInterestResult?.Links?.Self))
                {
                    continue;
                }

                var linkCompanyNumber = controllingInterestResult.Links.Self
                    .Replace("/company/", string.Empty)
                    .Replace("/persons-with-significant-control", string.Empty);

                var controllingInterestsItems = controllingInterestResult.Controllers
                    .Where(c => c.Kind == IndividualControllerKind &&
                                c.NaturesOfControl.Any() &&
                                string.IsNullOrEmpty(c.CeasedOn));

                var dictionaryItemsWithKeysMatchedCompanyNo =
                    activeAppointmentDictionary.Where(ad => ad.Key.Contains(linkCompanyNumber));
                var itemsWithKeysMatchedCompanyNo = dictionaryItemsWithKeysMatchedCompanyNo.ToList();

                if (!itemsWithKeysMatchedCompanyNo.Any())
                {
                    continue;
                }

                foreach (var item in controllingInterestsItems)
                {
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        continue;
                    }

                    var appointmentDictionaryEntry = itemsWithKeysMatchedCompanyNo
                        .FirstOrDefault(ad =>
                            item.Name != null &&
                            !string.IsNullOrEmpty(ad.Value.Name) &&
                            item.Name.ToLower().Contains(ad.Value.Name.ToLower()));

                    var appointment = appointmentDictionaryEntry.Value;

                    if (appointment == null)
                    {
                        continue;
                    }

                    var existingControllingInterest = controllingInterests.Where(ci =>
                        ci.Name == item.Name && ci.CompanyNumber == appointment.AppointedTo?.CompanyNumber);

                    if (existingControllingInterest.Any())
                    {
                        continue;
                    }

                    if (item.PercentageOfCapital < 10 && item.PercentageOfVotingRights < 10)
                    {
                        continue;
                    }

                    item.CompanyNumber = appointment.AppointedTo?.CompanyNumber;
                    item.CompanyName = appointment.AppointedTo?.CompanyName;

                    if (item.NameElements != null && !string.IsNullOrEmpty(appointment?.NameElements?.OtherForenames))
                    {
                        item.NameElements.OtherForenames = appointment.NameElements.OtherForenames;
                    }

                    if (string.IsNullOrEmpty(item.CompanyName) || string.IsNullOrEmpty(item.CompanyNumber))
                    {
                        continue;
                    }

                    var matchedCompany = matchedCompanyTaskListResults
                        .FirstOrDefault(r =>
                            r != null && r.CompanyNumber == item.CompanyNumber && r.CompanyName == item.CompanyName);
                    item.FirmReferenceNumber =
                        matchedCompany == null ? "Not available" : matchedCompany.FirmReferenceNo;

                    var companyProfile =
                        companyProfileTaskListResults.FirstOrDefault(r =>
                            r != null && r.CompanyNumber == item.CompanyNumber);
                    item.NatureOfBusiness = companyProfile?.SicCodes.FirstOrDefault();

                    controllingInterests.Add(item);
                }
            }

            return controllingInterests;
        }

        public Task<List<Controller>> GetIndividualControllersAsync(string companyNumber)
            => GetFirstLevelCompanyControllers(companyNumber, IndividualControllerKind);

        public async Task<List<Controller>> GetCorporateControllersRecursiveAsync(string companyNumber,
            int currentLevel = 1)
        {
            if (currentLevel > MaxRecurseLevel)
            {
                return new List<Controller>();
            }

            var results = await GetFirstLevelCompanyControllers(companyNumber, CorporateControllerKind);

            if (!results.Any())
            {
                return results;
            }

            var corporateControllers = results.Where(item => !string.IsNullOrEmpty(item.Name)).ToList();

            if (!corporateControllers.Any())
            {
                return results;
            }

            var companySearchTaskList = (
                from item in corporateControllers
                where !string.IsNullOrEmpty(item.Name)
                select SearchCompaniesWithDetailsAsync(item.Name, 20)
            ).ToList();

            var companySearchTaskListResults = await Task.WhenAll(companySearchTaskList);

            // combine all company details in 1 list
            var allCompanyDetailsResults = new List<Company>();
            companySearchTaskListResults.ToList().ForEach(r =>
            {
                var companyResults = r.ToList();

                if (companyResults.Any())
                {
                    allCompanyDetailsResults.AddRange(companyResults);
                }
            });

            var companyProfileTaskList = (
                    from item in corporateControllers
                    select allCompanyDetailsResults
                        .FirstOrDefault(c => string.Equals(RemoveLimitedWord(c.CompanyName),
                            RemoveLimitedWord(item.Name), StringComparison.CurrentCultureIgnoreCase))
                    into found
                    where found != null
                    select GetCompanyProfileAsync(found.CompanyNumber))
                .ToList();
            var companyProfileTaskListResults = await Task.WhenAll(companyProfileTaskList);

            foreach (var item in corporateControllers)
            {
                var found = allCompanyDetailsResults.FirstOrDefault(mc =>
                    string.Equals(RemoveLimitedWord(mc.CompanyName.Trim()), RemoveLimitedWord(item.Name?.Trim()),
                        StringComparison.CurrentCultureIgnoreCase));

                if (found == null)
                {
                    continue;
                }

                var companyProfile =
                    companyProfileTaskListResults.FirstOrDefault(cp => cp.CompanyNumber == found.CompanyNumber);

                if (companyProfile != null)
                {
                    item.NatureOfBusiness = companyProfile.SicCodes.FirstOrDefault();
                }

                item.FullAddress = found.Address;
                item.CompanyNumber = found.CompanyNumber;
                var individualControllers = await GetIndividualControllersAsync(item.CompanyNumber);

                if (individualControllers.Any())
                {
                    // get directorships & controllers
                    var directorshipsListTask = GetDirectorsAndAppointmentsAsync(companyNumber);
                    var controllingInterestsListTask = GetControllingInterestAsync(companyNumber);

                    await Task.WhenAll(directorshipsListTask, controllingInterestsListTask);

                    var directorshipsList = (await directorshipsListTask).ToList();
                    var controllingInterestsList = (await controllingInterestsListTask).ToList();

                    individualControllers.ForEach(individualController =>
                    {
                        individualController.CompanyNumber = item.CompanyNumber;

                        if (string.IsNullOrEmpty(individualController.Name))
                        {
                            return;
                        }

                        if (directorshipsList.Any())
                        {
                            var foundDirectorships = directorshipsList.FirstOrDefault(directorship =>
                                individualController.Name.Contains(directorship.FullName));
                            individualController.Directorships = foundDirectorships;
                        }

                        if (!controllingInterestsList.Any())
                        {
                            return;
                        }

                        var foundControllingInterests = controllingInterestsList.Where(controllingInterest =>
                            !string.IsNullOrEmpty(controllingInterest.Name) &&
                            individualController.Name.Contains(controllingInterest.Name)).ToList();

                        if (!foundControllingInterests.Any())
                        {
                            return;
                        }

                        var controllingInterests = foundControllingInterests.Select(controllingInterest =>
                            new CompanyControllingInterestDetails
                            {
                                CompanyName = controllingInterest.CompanyName,
                                CompanyNumber = controllingInterest.CompanyNumber,
                                CountryOfIncorporation = controllingInterest.Address?.Country,
                                CountryCode = GetCountryCodeFromCountryName(controllingInterest.Address?.Country),
                                FirmReferenceNumber = controllingInterest.FirmReferenceNumber,
                                NatureOfBusiness = controllingInterest.NatureOfBusiness,
                                PercentageOfCapital = controllingInterest.PercentageOfCapital,
                                PercentageOfVotingRights = controllingInterest.PercentageOfVotingRights
                            });

                        individualController.ControllingInterests = controllingInterests.ToList();
                    });

                    item.IndividualControllers = individualControllers;
                }

                var nextLevel = currentLevel + 1;
                var innerCorporateControllers =
                    await GetCorporateControllersRecursiveAsync(found.CompanyNumber, nextLevel);

                if (innerCorporateControllers.Any())
                {
                    item.CorporateControllers = innerCorporateControllers;
                }
            }

            // Get directors
            var directorsTaskList = (
                from item in corporateControllers
                where !string.IsNullOrEmpty(item.CompanyNumber)
                select GetCompanyActiveDirectorsAsync(item.CompanyNumber)).ToList();

            var directorsResults = await Task.WhenAll(directorsTaskList);

            foreach (var item in corporateControllers)
            {
                if (string.IsNullOrEmpty(item.CompanyNumber))
                {
                    continue;
                }

                var directors = directorsResults
                    .FirstOrDefault(directorResults => directorResults
                        .All(d => !string.IsNullOrEmpty(d.Links?.Self) && d.Links.Self.Contains(item.CompanyNumber)));

                item.Directors = directors?.ToList();
            }

            return results;
        }


        public async Task<IEnumerable<CompanyFilingHistoryItem>> GetCompanyFilingHistoryAsync(string companyNumber)
        {
            var endpoint = $"{BaseUrl}/company/{companyNumber}/filing-history";

            if (_memoryCache.Get(endpoint) is List<CompanyFilingHistoryItem> cachedValue)
            {
                return cachedValue;
            }

            var response = await GetRemoteAsync<CompanyFilingHistoryResult>(endpoint);
            var filingHistory = response.Items.ToList();

            _memoryCache.AddOrGetExisting(endpoint, filingHistory,
                DateTimeOffset.Now.AddDays(AppConstants.MemoryCacheOneDayDurationInMinutes));

            return filingHistory;
        }

        public async Task<CompanyFilingHistoryItem> GetCompanyFilingHistoryItemAsync(string companyNumber,
            string transactionId)
        {
            var endpoint = $"{BaseUrl}/company/{companyNumber}/filing-history/{transactionId}";

            if (_memoryCache.Get(endpoint) is CompanyFilingHistoryItem cachedValue)
            {
                return cachedValue;
            }

            var filingHistoryItem = await GetRemoteAsync<CompanyFilingHistoryItem>(endpoint);

            _memoryCache.AddOrGetExisting(endpoint, filingHistoryItem,
                DateTimeOffset.Now.AddDays(AppConstants.MemoryCacheOneDayDurationInMinutes));

            return filingHistoryItem;
        }

        public async Task<string> SaveCompanyFilingHistoryDocumentAsync(string documentId)
        {
            var endpoint = $"{DocumentBaseUrl}/document/{documentId}/content";

            var documentMetadataTask = GetDocumentMetadata(documentId);

            using var httpClient = new HttpClient();
            var documentTask = httpClient.SendAsync(CreateRequestMessageGet(endpoint));

            await Task.WhenAll(documentMetadataTask, documentTask);

            var documentMetadata = await documentMetadataTask;
            var pdfFileResponse = await documentTask;

            await using var file = await pdfFileResponse.Content.ReadAsStreamAsync();
            if (file.Length <= 0)
            {
                return string.Empty;
            }

            var container = _blobContainerClientService.BlobContainerClient;
            var createResponse = await container.CreateIfNotExistsAsync();

            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
            {
                await container.SetAccessPolicyAsync(PublicAccessType.Blob);
            }

            var blob = container.GetBlobClient($"DataStore/CompaniesHouse/Documents/{documentMetadata.Filename}.pdf");
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            await blob.UploadAsync(file, new BlobHttpHeaders { ContentType = "application/pdf" });

            return blob.Uri.ToString();
        }

        public async Task<CompanyProfileK?> GetCompanyProfileAsync(string companyNumber)
        {
            var endpoint = $"{BaseUrl}/company/{companyNumber}";

            if (_memoryCache.Get(endpoint) is CompanyProfileK cachedValue)
            {
                return cachedValue;
            }

            var response = await GetRemoteAsync(endpoint,
                async (response) => await HandleFailureAsync<CompanyProfileK>(endpoint, response));
            _memoryCache.AddOrGetExisting(endpoint, response,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return response;
        }

        private async Task<CompanyFilingHistoryDocument> GetDocumentMetadata(string documentId)
        {
            var endpoint = $"{DocumentBaseUrl}/document/{documentId}";
            var documentMetadata = await GetRemoteAsync<CompanyFilingHistoryDocument>(endpoint);

            return documentMetadata ?? new CompanyFilingHistoryDocument();
        }

        protected override HttpRequestMessage CreateRequestMessageGet(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Add("Authorization", GetBasicAuthorization());
            return request;
        }

        protected override HttpRequestMessage CreateRequestMessagePost(string endpoint, HttpContent httpContent)
        {
            throw new NotImplementedException();
        }

        private async Task<List<Controller>> GetFirstLevelCompanyControllers(string companyNumber, string kind)
        {
            var response = await GetPersonsWithSignificantControlAsync(companyNumber);

            if (response == null)
            {
                return new List<Controller>();
            }

            if (!response.Controllers.Any())
            {
                return new List<Controller>();
            }

            var controllers = response.Controllers
                .Where(c => c.Kind == kind && string.IsNullOrEmpty(c.CeasedOn)) // Include only active PSC
                .Where(c => c.PercentageOfCapital >= 10 || c.PercentageOfVotingRights >= 10)
                .ToList();

            if (kind != IndividualControllerKind)
            {
                return controllers;
            }

            // get directorships & controllers
            var directorshipsListTask = GetDirectorsAndAppointmentsAsync(companyNumber);
            var controllingInterestsListTask = GetControllingInterestAsync(companyNumber);

            await Task.WhenAll(directorshipsListTask, controllingInterestsListTask);

            var directorshipsList = (await directorshipsListTask).ToList();
            var controllingInterestsList = (await controllingInterestsListTask).ToList();

            controllers.ForEach(individualController =>
            {
                individualController.CompanyNumber = companyNumber;

                if (string.IsNullOrEmpty(individualController.Name))
                {
                    return;
                }

                if (directorshipsList.Any())
                {
                    var foundDirectorships = directorshipsList.FirstOrDefault(directorship =>
                        individualController.Name.Contains(directorship.FullName));
                    individualController.Directorships = foundDirectorships;
                }

                if (!controllingInterestsList.Any())
                {
                    return;
                }

                var foundControllingInterests = controllingInterestsList.Where(controllingInterest =>
                    !string.IsNullOrEmpty(controllingInterest.Name) &&
                    individualController.Name.Contains(controllingInterest.Name)).ToList();

                if (!foundControllingInterests.Any())
                {
                    return;
                }

                var controllingInterests = foundControllingInterests.Select(controllingInterest =>
                    new CompanyControllingInterestDetails
                    {
                        CompanyName = controllingInterest.CompanyName,
                        CompanyNumber = controllingInterest.CompanyNumber,
                        CountryOfIncorporation = controllingInterest.Address?.Country,
                        CountryCode = GetCountryCodeFromCountryName(controllingInterest.Address?.Country),
                        FirmReferenceNumber = controllingInterest.FirmReferenceNumber,
                        NatureOfBusiness = controllingInterest.NatureOfBusiness,
                        PercentageOfCapital = controllingInterest.PercentageOfCapital,
                        PercentageOfVotingRights = controllingInterest.PercentageOfVotingRights
                    });

                individualController.ControllingInterests = controllingInterests.ToList();
            });

            return controllers;
        }

        private static string RemoveLimitedWord(string? companyName)
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

        private static string GetBasicAuthorization()
        {
            var apiKey = Environment.GetEnvironmentVariable("CompanyHouseApiKey", EnvironmentVariableTarget.Process);
            var bytes = System.Text.Encoding.UTF8.GetBytes($"{apiKey}:");
            var authString = Convert.ToBase64String(bytes);
            return $"Basic {authString}";
        }

        private async Task<CompanyOfficerAppointmentDetails?> GetAppointmentsAsync(string appointmentsLink)
        {
            var endpoint = $"{BaseUrl}{appointmentsLink}";

            if (_memoryCache.Get(endpoint) is CompanyOfficerAppointmentDetails cachedValue)
            {
                return cachedValue;
            }

            var response = await GetRemoteAsync(endpoint,
                async (response) => await HandleFailureAsync<CompanyOfficerAppointmentDetails>(endpoint, response));
            _memoryCache.AddOrGetExisting(endpoint, response,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return response;
        }

        private async Task<ControllersResult?> GetPersonsWithSignificantControlAsync(string companyNumber)
        {
            var endpoint = $"{BaseUrl}/company/{companyNumber}/persons-with-significant-control";

            if (_memoryCache.Get(endpoint) is ControllersResult cachedValue)
            {
                return cachedValue;
            }

            var response = await GetRemoteAsync(endpoint,
                async (response) => await HandleFailureAsync<ControllersResult>(endpoint, response));

            _memoryCache.AddOrGetExisting(endpoint, response,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));
            return response;
        }

        private static string GetCountryCodeFromCountryName(string? countryName)
        {
            const string defaultCountryCode = "GB";

            if (string.IsNullOrEmpty(countryName))
            {
                return defaultCountryCode;
            }

            //ToDo.load a json mapper file here that list countries
            return countryName.ToLower() switch
            {
                "england" or "united kingdom" => defaultCountryCode,
                _ => defaultCountryCode, // TODO. to map other countries
            };
        }
    }
}