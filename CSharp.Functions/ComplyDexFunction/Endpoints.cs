using Azure.Storage.Blobs;
using Common.Infra;
using Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Common;
using System.Net.Sockets;
using System.Net;

namespace Api
{
    public partial class Endpoints
    {
        //values are put here right now, but soon will be moved to Vault
        private const string SignNowPassword = "3276T3ch";

        private readonly IGuideSchemaAndUiRepository _guideSchemaAndUiRepository;
        private readonly ISchemaRepository _schemaRepository;
        private readonly IUiSchemaRepository _uiSchemaRepository;
        private readonly ISchemaAnswerRepository _schemaAnswerRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IHtmlRepository _htmlRepository;
        private readonly ISignatureService _signatureService;
        private readonly IPdfService _pdfService;
        private readonly IWordToHtmlService _wordToHtmlService;
        private readonly IFcaPermissionsRepository _fcaPermissionsRepository;
        private readonly IFcaStatusRepository _fcaStatusRepository;
        private readonly IEmailQueueRepository _emailQueueRepository;
        private readonly ICompaniesHouseService _companiesHouseService;
        private readonly IFcaService _fcaService;
        private readonly IIcoService _icoService;
        private readonly IRatingsService _ratingsService;
        private readonly IAuth0Service _auth0Service;
        private readonly ISettingRepository _settingRepository;
        private readonly IEmailService _emailService;
        private readonly IMeetingRequestRepository _meetingRequestRepository;
        private readonly IDocGeneratorService _docGeneratorService;
        private readonly IBlobContainerService _blobContainerClientService;
        private readonly ICalendlyService _calendlyService;
        private readonly IGetAddressService _getAddressService;
        private readonly IInvitedUsersRepository _invitedUsersRepository;
        private readonly IFileUploaderService _fileUploaderService;
        private readonly ICommentRepository _commentRepository;
        private readonly IWebContentsScrapping _webContentsScrapping;
        private readonly IWebScrapsRepository _webScrapsRepository;
        private readonly IOrganizationalStructureRepository _organizationalStructureRepository;
        private readonly IProvidersRepository _providersRepository;
        private readonly IAffiliatesRepository _affiliatesRepository;
        private readonly IIntroducersRepository _introducersRepository;
        private readonly IFinancialPromotionRepository _financialPromotionRepository;
        private readonly IAppointedRepresentativeRepository _appointedRepresentativeRepository;
        private readonly IBaseFirmPermissionRepository _baseFirmPermissionRepository;
        private readonly IFcaRoleRepository _fcaRoleRepository;
        private readonly IProductMappingRepository _productMappingRepository;
        private readonly ICsvLookupService _csvLookupService;
        private readonly IWebScrapeService _webScrapeService;
        private readonly ICurrencyConversionService _currencyConversionService;
        private readonly ICurrencyConversionRepository _currencyConversionRepository;
        private readonly ICompanyHouseRepository _companyHouseRepository;
        private readonly ICustomerReplicationRepository _customerReplicationRepository;
        private readonly ICustomerFcaReplicationRepository _customerFcaReplicationRepository;
        private readonly BlobContainerClient _containerClient;
        private readonly string _azureAccountName;

        public Endpoints(IGuideSchemaAndUiRepository guideSchemaAndUiRepository,
            ISchemaRepository schemaRepository,
            IUiSchemaRepository uiSchemaRepository,
            ISchemaAnswerRepository schemaAnswerRepository,
            ICustomerRepository customerRepository,
            IHtmlRepository documentHtmlContentRepository,
            ISignatureService signatureService,
            IPdfService pdfService,
            IWordToHtmlService wordToHtmlService,
            IFcaPermissionsRepository fcaPermissionsRepository,
            IFcaStatusRepository fcaStatusRepository,
            IEmailQueueRepository emailQueueRepository,
            ICompaniesHouseService companiesHouseService,
            IFcaService fcaService,
            IIcoService icoService,
            IRatingsService ratingsService,
            IAuth0Service auth0Service,
            ISettingRepository settingRepository,
            IEmailService emailService,
            IMeetingRequestRepository meetingRequestRepository,
            IDocGeneratorService docGeneratorService,
            IBlobContainerService blobContainerClientService,
            ICalendlyService calendlyService,
            IGetAddressService getAddressService,
            IFileUploaderService fileUploaderService,
            ICommentRepository commentRepository,
            IWebContentsScrapping webContentsScrapping,
            IOrganizationalStructureRepository organizationalStructureRepository,
            IProvidersRepository providersRepository,
            IAffiliatesRepository affiliatesRepository,
            IIntroducersRepository introducersRepository,
            IWebScrapsRepository webScrapsRepository,
            IFinancialPromotionRepository financialPromotionRepository,
            ICsvLookupService csvLookupService,
            IWebScrapeService webScrapeService,
            ICurrencyConversionService currencyConversionService,
            ICurrencyConversionRepository currencyConversionRepository,
            IAppointedRepresentativeRepository appointedRepresentativeRepository,
            IBaseFirmPermissionRepository baseFirmPermissionRepository,
            IFcaRoleRepository fcaRoleRepository,
            ICompanyHouseRepository companyHouseRepository,
            IProductMappingRepository productMappingRepository,
            ICustomerReplicationRepository customerReplicationRepository,
            ICustomerFcaReplicationRepository customerFcaReplicationRepository)
        {
            _guideSchemaAndUiRepository = guideSchemaAndUiRepository;
            _schemaRepository = schemaRepository;
            _uiSchemaRepository = uiSchemaRepository;
            _schemaAnswerRepository = schemaAnswerRepository;
            _htmlRepository = documentHtmlContentRepository;
            _wordToHtmlService = wordToHtmlService;
            _fcaPermissionsRepository = fcaPermissionsRepository;
            _fcaStatusRepository = fcaStatusRepository;
            _emailQueueRepository = emailQueueRepository;
            _meetingRequestRepository = meetingRequestRepository;
            _companiesHouseService = companiesHouseService;
            _fcaService = fcaService;
            _icoService = icoService;
            _ratingsService = ratingsService;
            _emailService = emailService;
            _settingRepository = settingRepository;
            _pdfService = pdfService;
            _docGeneratorService = docGeneratorService;
            _fileUploaderService = fileUploaderService;
            _commentRepository = commentRepository;
            _webContentsScrapping = webContentsScrapping;
            _organizationalStructureRepository = organizationalStructureRepository;
            _providersRepository = providersRepository;
            _affiliatesRepository = affiliatesRepository;
            _introducersRepository = introducersRepository;
            _webScrapsRepository = webScrapsRepository;
            _csvLookupService = csvLookupService;
            _webScrapeService = webScrapeService;
            _financialPromotionRepository = financialPromotionRepository;
            _currencyConversionRepository = currencyConversionRepository;
            _appointedRepresentativeRepository = appointedRepresentativeRepository;
            _baseFirmPermissionRepository = baseFirmPermissionRepository;
            _fcaRoleRepository = fcaRoleRepository;
            _companyHouseRepository = companyHouseRepository;
            _productMappingRepository = productMappingRepository;
            _customerReplicationRepository = customerReplicationRepository;
            _customerFcaReplicationRepository = customerFcaReplicationRepository;

            _azureAccountName = Environment.GetEnvironmentVariable("AzureAccount", EnvironmentVariableTarget.Process);
            _auth0Service = auth0Service;

            var storageConnection =
                Environment.GetEnvironmentVariable("AzureStorageConnectionString", EnvironmentVariableTarget.Process);
            var containerName =
                Environment.GetEnvironmentVariable("BlobStorageContainerName", EnvironmentVariableTarget.Process);
            var callBackUrl =
                Environment.GetEnvironmentVariable("FunctionAppUrl", EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(callBackUrl) || string.IsNullOrEmpty(storageConnection) ||
                string.IsNullOrEmpty(containerName))
            {
                throw new NullReferenceException(
                    "Storage connection string, container name and callback url should be set");
            }

            _signatureService = signatureService;
            _signatureService.Register(new SignNowSignatureService(), callBackUrl);

            var blobServiceClient = new BlobServiceClient(storageConnection);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var clientId = Environment.GetEnvironmentVariable("AUTH0_CLIENT_ID", EnvironmentVariableTarget.Process) ??
                           throw new NullReferenceException($"constructor {nameof(Endpoints)} Auth vars null");
            var clientSecret =
                Environment.GetEnvironmentVariable("AUTH0_CLIENT_SECRET", EnvironmentVariableTarget.Process) ??
                throw new NullReferenceException($"constructor {nameof(Endpoints)} Auth vars null");
            var baseUrl = Environment.GetEnvironmentVariable("AUTH0_URL", EnvironmentVariableTarget.Process) ??
                          throw new NullReferenceException($"constructor {nameof(Endpoints)} Auth vars null");
            var apiToken = Environment.GetEnvironmentVariable("AUTH0_API_TOKEN", EnvironmentVariableTarget.Process) ??
                           throw new NullReferenceException($"constructor {nameof(Endpoints)} Auth vars null");
            _auth0Service.Register(clientId, clientSecret, baseUrl, apiToken);

            _calendlyService = calendlyService;
            var calendlyToken =
                Environment.GetEnvironmentVariable("CalendlyPersonalAccessToken", EnvironmentVariableTarget.Process) ??
                throw new NullReferenceException($"constructor {nameof(Endpoints)} Calendly token is null");
            _calendlyService.Register(calendlyToken);

            _getAddressService = getAddressService;
            var getAddressApiKey =
                Environment.GetEnvironmentVariable("GetAddressApiKey", EnvironmentVariableTarget.Process) ??
                throw new NullReferenceException($"constructor {nameof(Endpoints)} getaddress.io token is null");
            _getAddressService.Register(getAddressApiKey);

            var blobConnectionString =
                Environment.GetEnvironmentVariable("AzureStorageConnectionString", EnvironmentVariableTarget.Process) ??
                throw new NullReferenceException(
                    $"constructor {nameof(Endpoints)} AzureStorageConnectionString is null");
            var blobContainerName =
                Environment.GetEnvironmentVariable("BlobStorageContainerName", EnvironmentVariableTarget.Process) ??
                throw new NullReferenceException($"constructor {nameof(Endpoints)} BlobStorageContainerName is null");
            _blobContainerClientService = blobContainerClientService;
            _blobContainerClientService.Register(blobConnectionString, blobContainerName);

            _currencyConversionService = currencyConversionService;
            var getFixerApiKey =
                Environment.GetEnvironmentVariable("FixerApiKey", EnvironmentVariableTarget.Process) ??
                throw new NullReferenceException($"constructor {nameof(Endpoints)} FixerApiKey is null");
            _currencyConversionService.Register(getFixerApiKey);

            var isDisabledCheckingForMultipleSignUpsForSingleCompany =
                Environment.GetEnvironmentVariable("IsDisabledCheckingForMultipleSignUpsForSingleCompany",
                    EnvironmentVariableTarget.Process);

            if (bool.TryParse(isDisabledCheckingForMultipleSignUpsForSingleCompany,
                    out var isDisabledCheckingForMultipleSignUpsForSingleCompanyBool))
            {
                customerRepository.Register(isDisabledCheckingForMultipleSignUpsForSingleCompanyBool);
            }

            _customerRepository = customerRepository;

            //when false, the pdf co wont invoke because it has limit
            var isPdfCoEnabled = Environment.GetEnvironmentVariable("PdfCoEnabled", EnvironmentVariableTarget.Process);
            SetUrlsForDeletion();

            if (!bool.TryParse(isPdfCoEnabled, out var isPdfCo))
            {
                _pdfService.Register(new PdfCoPdfService(false));
            }

            _pdfService.Register(new PdfCoPdfService(isPdfCo));
        }

        [FunctionName(nameof(GetHeartBeat))]
        public IActionResult GetHeartBeat(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
            var workingUrl = $"{req.Scheme}://{req.Host}";
            var workingEnvironmentApi = "";

            if (workingUrl.ToLower().Contains("localhost"))
            {
                workingEnvironmentApi = "dev";
            }
            else if (workingUrl.ToLower().Contains("az-func-api-sun.azurewebsites.net"))
            {
                workingEnvironmentApi = "staging";
            }
            else if (workingUrl.ToLower().Contains("az-func-api-sun-live.azurewebsites.net"))
            {
                workingEnvironmentApi = "LIVE";
            }

            if (string.IsNullOrEmpty(workingEnvironmentApi))
            {
                throw new NullReferenceException("The working environment should have a value");
            }

            var dbName = Environment.GetEnvironmentVariable("Database", EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(dbName))
            {
                throw new NullReferenceException("The dbName should have a value");
            }

            return new OkObjectResult(
                $"Hello I'm alive. My api version is {version}. Hosted in '{_azureAccountName}'. It is connected to '{workingEnvironmentApi}' api and connected to '{dbName}' db.");
        }

        #region Json Forms Guide or original from Json Forms GitHub

        [FunctionName(nameof(GetGuideSchema))]
        public IActionResult GetGuideSchema(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var result = _guideSchemaAndUiRepository.GetSchema();
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GetGuideUiSchema))]
        public IActionResult GetGuideUiSchema(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var result = _guideSchemaAndUiRepository.GetUiSchema();
            return new OkObjectResult(result);
        }

        #endregion

        #region Json Forms

        [Obsolete]
        [FunctionName(nameof(InitializeSchemasAndUiSchemaAsync))]
        public async Task<IActionResult> InitializeSchemasAndUiSchemaAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var isSuccessSchema = await _schemaRepository.InitializeSchemasAsync();
            var isSuccessUiSchema = await _uiSchemaRepository.InitializeUiSchemasAsync();
            var isSuccessSchemaAnswers = await _schemaAnswerRepository.InitializeSchemaAnswersAsync();

            var responseText = isSuccessSchema switch
            {
                true when isSuccessUiSchema => "Schema and UiSchema have been initialized successfully!",
                true when !isSuccessUiSchema => "Schema has been initialized. UiSchema already existing!",
                false when isSuccessUiSchema => "UiSchema has been initialized. Schema already existing!",
                _ => "Schema and UiSchema already existing!"
            };

            responseText += isSuccessSchemaAnswers
                ? "Schema answers has been initialized"
                : "Schema answers already existing";
            return new OkObjectResult(responseText);
        }

        [Obsolete]
        [FunctionName(nameof(GetAllSchemaAsync))]
        public async Task<IActionResult> GetAllSchemaAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var schemasResult = await _schemaRepository.GetAllSchemaAsync();
            return new OkObjectResult(schemasResult);
        }

        [Obsolete]
        [FunctionName(nameof(GetSchemaAsync))]
        public async Task<IActionResult> GetSchemaAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{nameof(GetSchemaAsync)}/{{formNameKey}}")]
            HttpRequest req,
            ILogger log,
            string formNameKey)
        {
            var schemaResult = await _schemaRepository.GetSchemaAsync(formNameKey);
            return new OkObjectResult(schemaResult);
        }

        [Obsolete]
        [FunctionName(nameof(GetAllUiSchemaAsync))]
        public async Task<IActionResult> GetAllUiSchemaAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var schemasResult = await _uiSchemaRepository.GetAllUiSchemaAsync();
            return new OkObjectResult(schemasResult);
        }

        [Obsolete]
        [FunctionName(nameof(GetUiSchemaAsync))]
        public async Task<IActionResult> GetUiSchemaAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{nameof(GetUiSchemaAsync)}/{{formNameKey}}")]
            HttpRequest req,
            ILogger log,
            string formNameKey)
        {
            var schemaResult = await _uiSchemaRepository.GetUiSchemaAsync(formNameKey);
            return new OkObjectResult(schemaResult);
        }

        [Obsolete]
        [FunctionName(nameof(SaveSchemaAsync))]
        public async Task<IActionResult> SaveSchemaAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<SchemaModel>(requestBody);
            var existingSchemaModel = await _schemaRepository.GetSchemaAsync(data.FormNameKey);

            if (existingSchemaModel != null)
            {
                data.Id = existingSchemaModel.Id;
            }

            var postSchema = await _schemaRepository.AddOrUpdateSchemaAsync(data);
            return new OkObjectResult(postSchema);
        }

        [Obsolete]
        [FunctionName(nameof(GetAllSchemaAnswersAsync))]
        public async Task<IActionResult> GetAllSchemaAnswersAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var schemaAnswersResult = await _schemaAnswerRepository.GetSchemaAnswersAsync();
            return new OkObjectResult(schemaAnswersResult);
        }

        [Obsolete]
        [FunctionName(nameof(SaveSchemaAnswerAsync))]
        public async Task<IActionResult> SaveSchemaAnswerAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<SchemaAnswer>(requestBody);
            var postSchemaAnswer = await _schemaAnswerRepository.AddOrUpdateSchemaAnswerAsync(data);
            return new OkObjectResult(postSchemaAnswer);
        }

        [Obsolete]
        [FunctionName(nameof(SaveSchemaMultipleAsync))]
        public async Task<IActionResult> SaveSchemaMultipleAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<List<SchemaModel>>(requestBody);
            var postSchema = new List<SchemaModel>();

            foreach (var schemaModel in data)
            {
                var existingSchemaModel = await _schemaRepository.GetSchemaAsync(schemaModel.FormNameKey);

                if (existingSchemaModel != null)
                {
                    schemaModel.Id = existingSchemaModel.Id;
                }

                var savedSchema = await _schemaRepository.AddOrUpdateSchemaAsync(schemaModel);
                postSchema.Add(savedSchema);
            }

            return new OkObjectResult(postSchema);
        }

        [Obsolete]
        [FunctionName(nameof(SaveUiSchemaAsync))]
        public async Task<IActionResult> SaveUiSchemaAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<UiSchemaModel>(requestBody);
            var existingSchemaModel = await _uiSchemaRepository.GetUiSchemaAsync(data.FormNameKey);

            if (existingSchemaModel != null)
            {
                data.Id = existingSchemaModel.Id;
            }

            var postSchema = await _uiSchemaRepository.AddOrUpdateUiSchemaAsync(data);
            return new OkObjectResult(postSchema);
        }

        [Obsolete]
        [FunctionName(nameof(SaveUiSchemaMultipleAsync))]
        public async Task<IActionResult> SaveUiSchemaMultipleAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<List<UiSchemaModel>>(requestBody);

            var postUiSchema = new List<UiSchemaModel>();

            foreach (var uiSchema in data)
            {
                var existingSchemaModel = await _uiSchemaRepository.GetUiSchemaAsync(uiSchema.FormNameKey);

                if (existingSchemaModel != null)
                {
                    uiSchema.Id = existingSchemaModel.Id;
                }

                var savedUiSchema = await _uiSchemaRepository.AddOrUpdateUiSchemaAsync(uiSchema);
                postUiSchema.Add(savedUiSchema);
            }

            return new OkObjectResult(postUiSchema);
        }

        #endregion

        #region Customers Service

        [FunctionName(nameof(GetCustomersAsync))]
        public async Task<IActionResult> GetCustomersAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var customersResult = await _customerRepository.GetCustomersAsync();
            return new OkObjectResult(customersResult);
        }

        [FunctionName(nameof(GetCustomerByEmailAsync))]
        public async Task<IActionResult> GetCustomerByEmailAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{nameof(GetCustomerByEmailAsync)}/{{email}}")]
            HttpRequest req,
            ILogger log,
            string email)
        {
            var customersResult = await _customerRepository.GetCustomerByEmailAsync(email);
            return new OkObjectResult(customersResult);
        }

        [FunctionName(nameof(SaveCustomerAsync))]
        public async Task<IActionResult> SaveCustomerAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(requestBody);
            SetChangeInfo(customer, req);
            var wwwRootDirectory = context.FunctionAppDirectory;
            Customer savedCustomer;

            try
            {
                customer.IsLockProposalDocument = true;
                savedCustomer = await _customerRepository.SaveCustomerAsync(customer);
                var authUserTask = _auth0Service.CreateOnboardingUserAsync(savedCustomer);

                //This doc generation service is time consuming operation because we can't
                //make its internal method to operate the same time because it depends to
                //previous results
                var docGenerationTask = _docGeneratorService.GenerateDocumentAsync(savedCustomer,
                    DocumentNames.Proposal.ToString(), log,
                    _blobContainerClientService.BlobContainerClient, wwwRootDirectory);

                await Task.WhenAll(authUserTask, docGenerationTask);

                await authUserTask;
                await docGenerationTask;
            }
            catch
            {
                customer.IsLockProposalDocument = false;
                await _customerRepository.SaveCustomerAsync(customer);
                throw;
            }

            return new OkObjectResult(savedCustomer);
        }

        [FunctionName(nameof(CheckAndUpdateMeetingRequestIfFoundAsync))]
        public async Task<IActionResult> CheckAndUpdateMeetingRequestIfFoundAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var calendlyEmail = req.Query["calendlyEmail"];
            var customerEmail = req.Query["customerEmail"];
            var eventTypeId = req.Query["eventTypeId"];
            var startDateTime = long.Parse(req.Query["startDateTime"].ToString());

            var customersResult =
                await _meetingRequestRepository.CheckAndUpdateMeetingRequestIfFoundAsync(calendlyEmail, customerEmail,
                    eventTypeId, startDateTime);
            return new OkObjectResult(customersResult);
        }

        // not used in frontend just testing in postman
        [FunctionName(nameof(GetColleaguesAsync))]
        public async Task<IActionResult> GetColleaguesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{nameof(GetColleaguesAsync)}/{{email}}")]
            HttpRequest req,
            ILogger log,
            string email)
        {
            var customersResult = await _customerRepository.GetColleaguesAsync(email);
            return new OkObjectResult(customersResult);
        }

        [FunctionName(nameof(PostInviteUserAsync))]
        public async Task<IActionResult> PostInviteUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(requestBody);
            var customersResult = await _invitedUsersRepository.InviteUser(customer);
            return new OkObjectResult(customersResult);
        }

        [FunctionName(nameof(CheckIfCompanyHasProposalSignedAsync))]
        public async Task<IActionResult> CheckIfCompanyHasProposalSignedAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(CheckIfCompanyHasProposalSignedAsync)}/{{companyNumber}}")]
            HttpRequest req,
            ILogger log,
            string companyNumber)
        {
            var hasSignedProposal =
                await _customerRepository.CheckIfCompanyHasSignedProposalAlreadyAsync(companyNumber);
            return new OkObjectResult(hasSignedProposal);
        }

        #endregion

        #region Settings

        [FunctionName(nameof(InitializeSettingsAsync))]
        public async Task<IActionResult> InitializeSettingsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            await _settingRepository.InitializeSettingsAsync();
            return new OkObjectResult("Initializing Settings successful!");
        }

        [FunctionName(nameof(SaveSettingAsync))]
        public async Task<IActionResult> SaveSettingAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Setting>(requestBody);
            var settingResult = await _settingRepository.AddOrUpdateSettingAsync(data);
            return new OkObjectResult(settingResult);
        }

        [FunctionName(nameof(SaveSettingsMultipleAsync))]
        public async Task<IActionResult> SaveSettingsMultipleAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var settings = JsonConvert.DeserializeObject<List<Setting>>(requestBody);
            var settingResult = new List<Setting>();

            await _settingRepository.DeleteAllSettingsAsync();

            foreach (var setting in settings)
            {
                var existingSetting = await _settingRepository.GetSettingByIdAsync(setting.Id);

                if (existingSetting != null)
                {
                    setting.Id = existingSetting.Id;
                }

                var savedSetting =
                    await _settingRepository.AddOrUpdateSettingAsync(setting);
                settingResult.Add(savedSetting);
            }

            return new OkObjectResult(settingResult);
        }

        [FunctionName(nameof(GetAllSettingsAsync))]
        public async Task<IActionResult> GetAllSettingsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var settingsResult = await _settingRepository.GetAllSettingAsync();
            return new OkObjectResult(settingsResult);
        }

        [FunctionName(nameof(GetSettingByIdAsync))]
        public async Task<IActionResult> GetSettingByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{nameof(GetSettingByIdAsync)}/{{id}}")]
            HttpRequest req,
            ILogger log,
            string id)
        {
            var settingResult = await _settingRepository.GetSettingByIdAsync(id);
            return new OkObjectResult(settingResult);
        }

        [FunctionName(nameof(GetSettingByKeyAsync))]
        public async Task<IActionResult> GetSettingByKeyAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{nameof(GetSettingByKeyAsync)}/{{key}}")]
            HttpRequest req,
            ILogger log,
            string key)
        {
            var settingResult = await _settingRepository.GetSettingByKeyAsync(key);
            return new OkObjectResult(settingResult);
        }

        [FunctionName(nameof(DeleteSettingAsync))]
        public async Task<IActionResult> DeleteSettingAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = $"{nameof(DeleteSettingAsync)}/{{id}}")]
            HttpRequest req,
            ILogger log,
            string id)
        {
            var isSuccess = await _settingRepository.DeleteSettingAsync(id);
            return new OkObjectResult(isSuccess);
        }

        #endregion

        #region Email Queue Service. This is for Camunda could be removed

        [FunctionName(nameof(GetEmailsInQueueAsync))]
        public async Task<IActionResult> GetEmailsInQueueAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var emailQueueResult = await _emailQueueRepository.GetAllEmailsInQueueAsync();
            return new OkObjectResult(emailQueueResult);
        }

        [FunctionName(nameof(SaveEmailToQueueAsync))]
        public async Task<IActionResult> SaveEmailToQueueAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var emails = JsonConvert.DeserializeObject<IEnumerable<EmailQueue>>(requestBody);
            var isSuccess = await _emailQueueRepository.SaveEmailInQueueAsync(emails);
            return new OkObjectResult(isSuccess);
        }

        #endregion

        #region Auth0 Service 3rd Party

        [FunctionName(nameof(GetAccessTokenForSignupAsync))]
        public async Task<IActionResult> GetAccessTokenForSignupAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var accessToken = await _auth0Service.GetAccessTokenForSignUpAsync();
            return new OkObjectResult(accessToken);
        }

        //not used in frontend just testing in postman
        [FunctionName(nameof(CreateUserAsync))]
        public async Task<IActionResult> CreateUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(requestBody);
            var newAuth0User = await _auth0Service.CreateOnboardingUserAsync(customer);
            return new OkObjectResult(newAuth0User);
        }

        [FunctionName(nameof(ChangePasswordAsync))]
        public async Task<IActionResult> ChangePasswordAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var credentials = JsonConvert.DeserializeObject<OnboardingChangePasswordPayload>(requestBody);
            var email = credentials.Email;
            var result = await _auth0Service.ChangePasswordAsync(
                email ?? throw new NullReferenceException($"Email should not be null in {nameof(ChangePasswordAsync)}"),
                credentials.Password ??
                throw new NullReferenceException($"Password should not be null in {nameof(ChangePasswordAsync)}"));

            if (credentials.OnboardingType == OnboardingTypes.Ar.ToString())
            {
                var existingArCustomer =
                    await _appointedRepresentativeRepository.GetAppointedRepresentativeByEmailAsync(email) ??
                    throw new Exception("User should be existing by now.");
                existingArCustomer.IsUserPasswordSet = true;
                await _appointedRepresentativeRepository.SaveOrUpdateAppointedRepresentativeAsync(existingArCustomer);
                SetChangeInfo(existingArCustomer, req);
            }
            else if (credentials.OnboardingType == OnboardingTypes.Employee.ToString())
            {
                var existingCustomerEmployee =
                    await _organizationalStructureRepository.GetEmployeeByEmailAsync(email)
                    ?? throw new Exception("User should be existing by now.");
                existingCustomerEmployee.IsUserPasswordSet = true;
                await _organizationalStructureRepository.SaveOrUpdateEmployeeAsync(existingCustomerEmployee);
                SetChangeInfo(existingCustomerEmployee, req);
            }
            else if (credentials.OnboardingType == OnboardingTypes.Provider.ToString())
            {
                var existingProvider =
                    await _providersRepository.GetProviderByEmailAsync(email)
                    ?? throw new Exception("User should be existing by now.");
                existingProvider.IsUserPasswordSet = true;
                await _providersRepository.SaveOrUpdateProvidersAsync(existingProvider);
                SetChangeInfo(existingProvider, req);
            }
            else if (credentials.OnboardingType == OnboardingTypes.Introducer.ToString())
            {
                var existingIntroducer =
                    await _introducersRepository.GetIntroducerByEmailAsync(email)
                    ?? throw new Exception("User should be existing by now.");
                existingIntroducer.IsUserPasswordSet = true;
                await _introducersRepository.SaveOrUpdateIntroducersAsync(existingIntroducer);
                SetChangeInfo(existingIntroducer, req);
            }
            else if (credentials.OnboardingType == OnboardingTypes.Affiliate.ToString())
            {
                var existingAffiliate =
                    await _affiliatesRepository.GetAffiliateByEmailAsync(email)
                    ?? throw new Exception("User should be existing by now.");
                existingAffiliate.IsUserPasswordSet = true;
                await _affiliatesRepository.SaveOrUpdateAffiliatesAsync(existingAffiliate);
                SetChangeInfo(existingAffiliate, req);
            }
            else
            {
                var existingCustomer = await _customerRepository.GetCustomerByEmailAsync(email) ??
                                       throw new Exception("User should be existing by now.");
                existingCustomer.IsUserPasswordSet = true;
                SetChangeInfo(existingCustomer, req);
                await _customerRepository.SaveCustomerAsync(existingCustomer);
            }

            return new OkObjectResult(result.IsSuccessStatusCode);
        }

        #endregion

        #region Organizational Structures

        [FunctionName(nameof(GetEmployeesAsync))]
        public async Task<IActionResult> GetEmployeesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{nameof(GetEmployeesAsync)}/{{companyNumber}}")]
            HttpRequest req,
            ILogger log,
            string companyNumber)
        {
            var employeeResult = await _organizationalStructureRepository.GetEmployeesAsync(companyNumber);
            return new OkObjectResult(employeeResult);
        }

        [FunctionName(nameof(GetEmployeeByEmailAsync))]
        public async Task<IActionResult> GetEmployeeByEmailAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetEmployeeByEmailAsync)}/{{email}}")]
            HttpRequest req,
            ILogger log,
            string email)
        {
            var customerArResult =
                await _organizationalStructureRepository.GetEmployeeByEmailAsync(email);
            return new OkObjectResult(customerArResult);
        }

        // TODO. To complete CRUD
        [FunctionName(nameof(SaveOrUpdateEmployeeAsync))]
        public async Task<IActionResult> SaveOrUpdateEmployeeAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var employee = JsonConvert.DeserializeObject<Employee>(requestBody);
            var savedEmployee = await _organizationalStructureRepository.SaveOrUpdateEmployeeAsync(employee);

            if (savedEmployee.ProfileStatus.Equals(ProfileStatuses.Full.ToString()))
            {
                await _auth0Service.CreateOnboardingUserAsync(savedEmployee);
            }

            return new OkObjectResult(savedEmployee);
        }

        #endregion

        #region Provider

        [FunctionName(nameof(GetProviderByEmailAsync))]
        public async Task<IActionResult> GetProviderByEmailAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetProviderByEmailAsync)}/{{email}}")]
            HttpRequest req,
            ILogger log,
            string email)
        {
            var providerResult =
                await _providersRepository.GetProviderByEmailAsync(email);
            return new OkObjectResult(providerResult);
        }

        [FunctionName(nameof(GetProviderByCustomerIdAsync))]
        public async Task<IActionResult> GetProviderByCustomerIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetProviderByCustomerIdAsync)}/{{customerId}}")]
            HttpRequest req,
            ILogger log,
            string customerId)
        {
            var providerResult =
                await _providersRepository.GetProvidersByCustomerIdAsync(customerId);
            return new OkObjectResult(providerResult);
        }

        [FunctionName(nameof(DeleteProviderAsync))]
        public async Task<IActionResult> DeleteProviderAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete",
                Route = $"{nameof(DeleteProviderAsync)}/{{id}}")]
            HttpRequest req,
            ILogger log,
            string id)
        {
            var providerResult =
                await _providersRepository.DeleteProviderAsync(id);
            return new OkObjectResult(providerResult);
        }

        [FunctionName(nameof(SaveOrUpdateProvidersAsync))]
        public async Task<IActionResult> SaveOrUpdateProvidersAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var provider = JsonConvert.DeserializeObject<Providers>(requestBody);
            var savedProvider = await _providersRepository.SaveOrUpdateProvidersAsync(provider);

            if (savedProvider.ProfileStatus.Equals(ProfileStatuses.Full.ToString()))
            {
                await _auth0Service.CreateOnboardingUserAsync(savedProvider);
            }

            return new OkObjectResult(savedProvider);
        }

        #endregion

        #region Introducer

        [FunctionName(nameof(GetIntroducerByEmailAsync))]
        public async Task<IActionResult> GetIntroducerByEmailAsync(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetIntroducerByEmailAsync)}/{{email}}")]
            HttpRequest req,
           ILogger log,
           string email)
        {
            var introducerResult =
                await _introducersRepository.GetIntroducerByEmailAsync(email);
            return new OkObjectResult(introducerResult);
        }

        [FunctionName(nameof(GetIntroducerByCustomerIdAsync))]
        public async Task<IActionResult> GetIntroducerByCustomerIdAsync(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetIntroducerByCustomerIdAsync)}/{{customerId}}")]
            HttpRequest req,
           ILogger log,
           string customerId)
        {
            var introducerResult =
                await _introducersRepository.GetIntroducersByCustomerIdAsync(customerId);
            return new OkObjectResult(introducerResult);
        }

        [FunctionName(nameof(DeleteIntroducerAsync))]
        public async Task<IActionResult> DeleteIntroducerAsync(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete",
                Route = $"{nameof(DeleteIntroducerAsync)}/{{id}}")]
            HttpRequest req,
           ILogger log,
           string id)
        {
            var introducerResult =
                await _introducersRepository.DeleteIntroducerAsync(id);
            return new OkObjectResult(introducerResult);
        }

        [FunctionName(nameof(SaveOrUpdateIntroducersAsync))]
        public async Task<IActionResult> SaveOrUpdateIntroducersAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var introducer = JsonConvert.DeserializeObject<Introducer>(requestBody);
            var savedIntroducer = await _introducersRepository.SaveOrUpdateIntroducersAsync(introducer);

            if (savedIntroducer.ProfileStatus.Equals(ProfileStatuses.Full.ToString()))
            {
                await _auth0Service.CreateOnboardingUserAsync(savedIntroducer);
            }

            return new OkObjectResult(savedIntroducer);
        }

        #endregion

        #region Affiliate

        [FunctionName(nameof(GetAffiliateByEmailAsync))]
        public async Task<IActionResult> GetAffiliateByEmailAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetAffiliateByEmailAsync)}/{{email}}")]
            HttpRequest req,
            ILogger log,
            string email)
        {
            var providerResult =
                await _affiliatesRepository.GetAffiliateByEmailAsync(email);
            return new OkObjectResult(providerResult);
        }

        [FunctionName(nameof(GetAffiliateByCustomerIdAsync))]
        public async Task<IActionResult> GetAffiliateByCustomerIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetAffiliateByCustomerIdAsync)}/{{customerId}}")]
            HttpRequest req,
            ILogger log,
            string customerId)
        {
            var providerResult =
                await _affiliatesRepository.GetAffiliatesByCustomerIdAsync(customerId);
            return new OkObjectResult(providerResult);
        }

        [FunctionName(nameof(DeleteAffiliateAsync))]
        public async Task<IActionResult> DeleteAffiliateAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete",
                Route = $"{nameof(DeleteAffiliateAsync)}/{{id}}")]
            HttpRequest req,
            ILogger log,
            string id)
        {
            var providerResult =
                await _affiliatesRepository.DeleteAffiliateAsync(id);
            return new OkObjectResult(providerResult);
        }

        [FunctionName(nameof(SaveOrUpdateAffiliatesAsync))]
        public async Task<IActionResult> SaveOrUpdateAffiliatesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var provider = JsonConvert.DeserializeObject<Affiliate>(requestBody);
            var savedAffiliate = await _affiliatesRepository.SaveOrUpdateAffiliatesAsync(provider);

            if (savedAffiliate.ProfileStatus.Equals(ProfileStatuses.Full.ToString()))
            {
                await _auth0Service.CreateOnboardingUserAsync(savedAffiliate);
            }

            return new OkObjectResult(savedAffiliate);
        }

        #endregion

        #region Financial Promotions

        [FunctionName(nameof(GetFinancialPromotionsAsync))]
        public async Task<IActionResult> GetFinancialPromotionsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post",
                Route = $"{nameof(GetFinancialPromotionsAsync)}/{{customerId}}")]
            HttpRequest req,
            ILogger log,
            string customerId)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var filter = JsonConvert.DeserializeObject<FinancialPromotionFilter>(requestBody) ??
                         new FinancialPromotionFilter();

            var conditions = new List<Func<FinancialPromotion, bool>>();
            if (filter.MediaOutlets.Any())
            {
                conditions.Add(fp => filter.MediaOutlets.Contains(fp.MediaOutlet));
            }

            if (filter.Types.Any())
            {
                conditions.Add(fp => filter.Types.Contains(fp.Type));
            }

            if (filter.ApprovalTypes.Any())
            {
                conditions.Add(fp => filter.ApprovalTypes.Contains(fp.ApprovalStatus.ToString()));
            }

            if (!string.IsNullOrEmpty(filter.ContentStatus))
            {
                var isLive = filter.ContentStatus.Equals("Live", StringComparison.InvariantCultureIgnoreCase);
                conditions.Add(fp => fp.IsLive == isLive);
            }

            var financialPromotionResult = await _financialPromotionRepository.GetFinancialPromotionsAsync(customerId);
            var allFinancialPromotions = financialPromotionResult.ToList();

            var financialPromotions = allFinancialPromotions.Where(fp => conditions.All(c => c(fp)))
                .SortByProperty(filter.SortPropertyName, filter.SortDirection == "desc")
                .Paginate(filter.PageNumber, filter.PageSize);

            var financialPromotionsObject = new
            {
                Data = financialPromotions,
                Total = allFinancialPromotions.Count
            };

            return new OkObjectResult(financialPromotionsObject);
        }

        [FunctionName(nameof(GetFinancialPromotionAsync))]
        public async Task<IActionResult> GetFinancialPromotionAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetFinancialPromotionAsync)}/{{id}}")]
            HttpRequest req,
            ILogger log,
            string id)
        {
            var financialPromotionResult = await _financialPromotionRepository.GetFinancialPromotionByIdAsync(id);
            return new OkObjectResult(financialPromotionResult);
        }

        [FunctionName(nameof(SaveOrUpdateFinancialPromotionAsync))]
        public async Task<IActionResult> SaveOrUpdateFinancialPromotionAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var financialPromotion = JsonConvert.DeserializeObject<FinancialPromotion>(requestBody);
            var financialPromotionResult =
                await _financialPromotionRepository.SaveOrUpdateFinancialPromotionAsync(financialPromotion);
            return new OkObjectResult(financialPromotionResult);
        }

        [FunctionName(nameof(DeleteFinancialPromotionAsync))]
        public async Task<IActionResult> DeleteFinancialPromotionAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", "post",
                Route = $"{nameof(DeleteFinancialPromotionAsync)}/{{id}}")]
            HttpRequest req, ILogger log, string id)
        {
            var financialPromotionResult = await _financialPromotionRepository.DeleteFinancialPromotionByIdAsync(id);
            return new OkObjectResult(financialPromotionResult);
        }

        #endregion

        #region Appointed Representatives

        [FunctionName(nameof(GetAppointedRepresentativesAsync))]
        public async Task<IActionResult> GetAppointedRepresentativesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post",
                Route = $"{nameof(GetAppointedRepresentativesAsync)}/{{customerId}}")]
            HttpRequest req,
            ILogger log,
            string customerId)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var filter = JsonConvert.DeserializeObject<AppointedRepresentativeFilter>(requestBody) ??
                         new AppointedRepresentativeFilter();

            var conditions = new List<Func<AppointedRepresentative, bool>>();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                conditions.Add(ar => ar.Name == filter.Name);
            }

            if (!string.IsNullOrEmpty(filter.Search))
            {
                conditions.Add(ar =>
                    !string.IsNullOrEmpty(ar.Name) && ar.Name.ToLower().Contains(filter.Search.ToLower())
                );
            }

            if (!string.IsNullOrEmpty(filter.Website))
            {
                conditions.Add(ar => ar.Website == filter.Website);
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                var statusFilter = (ArStatus)Enum.Parse(typeof(ArStatus), filter.Status);
                conditions.Add(ar => ar.Status == statusFilter);
            }

            var appointedRepresentativeResult =
                await _appointedRepresentativeRepository.GetAppointedRepresentativesAsync(customerId);
            var allAppointedRepresentatives = appointedRepresentativeResult.ToList();

            var appointedRepresentatives = allAppointedRepresentatives.Where(fp => conditions.All(c => c(fp)))
                .SortByProperty(filter.SortPropertyName, filter.SortDirection == "desc")
                .Paginate(filter.PageNumber, filter.PageSize);

            var appointedRepresentativesObject = new
            {
                Data = appointedRepresentatives,
                Total = allAppointedRepresentatives.Count
            };

            return new OkObjectResult(appointedRepresentativesObject);
        }

        [FunctionName(nameof(GetAppointedRepresentativeAsync))]
        public async Task<IActionResult> GetAppointedRepresentativeAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetAppointedRepresentativeAsync)}/{{id}}")]
            HttpRequest req,
            ILogger log,
            string id)
        {
            var appointedRepresentativeResult =
                await _appointedRepresentativeRepository.GetAppointedRepresentativeByIdAsync(id);
            return new OkObjectResult(appointedRepresentativeResult);
        }

        [FunctionName(nameof(GetAppointedRepresentativeByEmailAsync))]
        public async Task<IActionResult> GetAppointedRepresentativeByEmailAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = $"{nameof(GetAppointedRepresentativeByEmailAsync)}/{{email}}")]
            HttpRequest req,
            ILogger log,
            string email)
        {
            var appointedRepresentativeResult =
                await _appointedRepresentativeRepository.GetAppointedRepresentativeByEmailAsync(email);
            return new OkObjectResult(appointedRepresentativeResult);
        }

        [FunctionName(nameof(SaveOrUpdateAppointedRepresentativeAsync))]
        public async Task<IActionResult> SaveOrUpdateAppointedRepresentativeAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var appointedRepresentative = JsonConvert.DeserializeObject<AppointedRepresentative>(requestBody);
            var savedAppointedRepresentative =
                await _appointedRepresentativeRepository.SaveOrUpdateAppointedRepresentativeAsync(
                    appointedRepresentative);

            if (savedAppointedRepresentative.ProfileStatus.Equals(ProfileStatuses.Full.ToString()))
            {
                await _auth0Service.CreateOnboardingUserAsync(savedAppointedRepresentative);
            }

            return new OkObjectResult(savedAppointedRepresentative);
        }

        [FunctionName(nameof(DeleteAppointedRepresentativeAsync))]
        public async Task<IActionResult> DeleteAppointedRepresentativeAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", "post",
                Route = $"{nameof(DeleteAppointedRepresentativeAsync)}/{{id}}")]
            HttpRequest req, ILogger log, string id)
        {
            var appointedRepresentativeResult =
                await _appointedRepresentativeRepository.DeleteAppointedRepresentativeByIdAsync(id);
            return new OkObjectResult(appointedRepresentativeResult);
        }

        #endregion

        #region Permission Mappings

        [FunctionName(nameof(SaveOrUpdateBaseFirmPermissionAsync))]
        public async Task<IActionResult> SaveOrUpdateBaseFirmPermissionAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var baseFirmPermission = JsonConvert.DeserializeObject<BaseFirmPermission>(requestBody);
            var saveResult =
                await _baseFirmPermissionRepository.SaveOrUpdateBaseFirmPermissionAsync(baseFirmPermission);
            return new OkObjectResult(saveResult);
        }

        #endregion

        #region Misc

        [FunctionName(nameof(SaveOrUpdateFcaRoleAsync))]
        public async Task<IActionResult> SaveOrUpdateFcaRoleAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var fcaRole = JsonConvert.DeserializeObject<FcaRole>(requestBody);
            var saveResult = await _fcaRoleRepository.SaveOrUpdateFcaRoleAsync(fcaRole);
            return new OkObjectResult(saveResult);
        }

        //just put here if endpoints has no grouping yet
        [FunctionName(nameof(GetCustomerProductsAsync))]
        public async Task<IActionResult> GetCustomerProductsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var email = req.Query["email"];
            var customer = await _customerRepository.GetCustomerByEmailAsync(email);
            var customerPermissions = customer?.CustomerPermissions.Select(cp => cp.SubPermissionName).ToList();

            var productMappings = (await _productMappingRepository.GetProductMappingsAsync()).ToList();
            var pageNames = productMappings.Where(pm => pm.EnablerType == EnablerType.Page).DistinctBy(d => d.Enabler)
                .Select(s => s.Enabler);

            var customerProducts = new List<CustomerProduct>();

            foreach (var pageName in pageNames)
            {
                var customerProduct = new CustomerProduct { PageName = pageName };
                var mappingCategories =
                    productMappings.Where(pm => pm.Enabler == pageName).DistinctBy(d => d.ProductName)
                        .OrderBy(o => o.SortOrder);
                var pageCategories = mappingCategories.Select(mappingCategory => new CategoryObject
                {
                    Name = mappingCategory.ProductName,
                    DisplayText = mappingCategory.DisplayText,
                    Products = productMappings
                        .Where(pm =>
                            customerPermissions != null && customerPermissions.Contains(pm.Enabler) &&
                            pm.CategoryName == mappingCategory.ProductName).Select(s => new ProductObject
                            { Name = s.ProductName, DisplayText = s.DisplayText, SortOrder = s.SortOrder })
                        .DistinctBy(d => d.Name).OrderBy(o => o.SortOrder).ToList()
                }).ToList();

                customerProduct.Categories = pageName == "Appointed Representatives"
                    ? pageCategories
                    : pageCategories.Where(c => c.Products != null && c.Products.Any()).ToList();

                customerProducts.Add(customerProduct);
            }

            return new OkObjectResult(customerProducts);
        }

        [FunctionName(nameof(SaveOrUpdateProductMappingAsync))]
        public async Task<IActionResult> SaveOrUpdateProductMappingAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var productMapping = JsonConvert.DeserializeObject<ProductMapping>(requestBody);
            var saveResult = await _productMappingRepository.SaveOrUpdateProductMappingAsync(productMapping);
            return new OkObjectResult(saveResult);
        }

        #endregion

        private static string CleanStringFromExcessDoubleQuotes(string input)
        {
            // For some reasons, strings coming from Vue app has excess double quotes. e.g. ("\"test\"")
            return input.Replace("\"", string.Empty);
        }

        // not used in frontend just testing in postman
        [FunctionName(nameof(SendSmtpEmailAsync))]
        public IActionResult SendSmtpEmailAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var recipient = req.Query["recipient"];
            var subject = req.Query["subject"];
            var body = req.Query["body"];
            var webhookSubscription = _emailService.SendEmail(recipient, subject, body);
            return new OkObjectResult(webhookSubscription);
        }

        private static void SetChangeInfo(ChangeInfo customer, HttpRequest req)
        {
            customer.ChangedOn = DateHelper.GetCurrentDateTimeInEpoch();
            customer.IpAddress = req.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}