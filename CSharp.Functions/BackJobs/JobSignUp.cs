using Azure.Storage.Blobs;
using Common.Infra;
using Common.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace BackJobs
{
    public class JobSignUp
    {
        //values are put here right now, but soon will be moved to Vault
        private const string Password = "3276T3ch";
        private static readonly SemaphoreSlim Semaphore = new(1, 1);
        private static bool _isAllTasksCompleted = true;

        private readonly ICustomerRepository _customerRepository;
        private readonly IAppointedRepresentativeRepository _appointedRepresentativeRepository;
        private readonly IOrganizationalStructureRepository _organizationalStructureRepository;
        private readonly IProvidersRepository _providersRepository;
        private readonly IIntroducersRepository _introducersRepository;
        private readonly IEmailService _emailService;
        private readonly ISignatureService _signatureService;
        private readonly ISaveFieldsRepository _saveFieldsRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly ICalendlyService _calendlyService;
        private readonly BlobContainerClient _containerClient;
        private readonly bool _isEnable = true;

        public JobSignUp(ICustomerRepository customerRepository,
            IAppointedRepresentativeRepository appointedRepresentativeRepository,
            IOrganizationalStructureRepository organizationalStructureRepository,
            IProvidersRepository providersRepository,
            IIntroducersRepository introducersRepository,
            IEmailService emailService,
            ISignatureService signatureService,
            ISaveFieldsRepository saveFieldsRepository,
            ISettingRepository settingRepository,
            ICalendlyService calendlyService,
            IBlobContainerService blobContainerClientService)
        {
            _customerRepository = customerRepository;
            _appointedRepresentativeRepository = appointedRepresentativeRepository;
            _organizationalStructureRepository = organizationalStructureRepository;
            _providersRepository = providersRepository;
            _introducersRepository = introducersRepository;
            _saveFieldsRepository = saveFieldsRepository;
            _settingRepository = settingRepository;
            _emailService = emailService;

            _calendlyService = calendlyService;
            var calendlyToken =
                Environment.GetEnvironmentVariable("CalendlyPersonalAccessToken", EnvironmentVariableTarget.Process) ??
                throw new NullReferenceException($"constructor {nameof(JobSignUp)} Calendly token is null");
            _calendlyService.Register(calendlyToken);

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

            var enabled =
                Environment.GetEnvironmentVariable("IsEnable", EnvironmentVariableTarget.Process);

            if (bool.TryParse(enabled, out var isEnable))
            {
                _isEnable = isEnable;
            }

            blobContainerClientService.Register(storageConnection, containerName);
        }

        [FunctionName("JobSignUp")]
        public async Task Run([TimerTrigger("%ScheduleExpression%")] TimerInfo myTimer, ILogger log)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
            log.LogInformation("Backjobs function executed at {Now} with version {Version}", DateTime.Now, version);

            if (!_isEnable)
            {
                log.LogWarning($"{nameof(JobSignUp)}: Back Jobs is DISABLED.");
                return;
            }

            await Semaphore.WaitAsync();

            if (!_isAllTasksCompleted)
            {
                Semaphore.Release();
                log.LogWarning("we will run again in the next round when all tasks are completed");
                return;
            }

            _isAllTasksCompleted = false;
            Semaphore.Release();
            var proposalTaskStatus = false;
            var proposalFollowUpTaskStatus = false;
            var sendInvitationToCustomerArTaskStatus = false;
            var sendInvitationToCustomerEmployeeTaskStatus = false;
            var sendInvitationToCustomerProviderTaskStatus = false;
            var sendInvitationToCustomerIntroducerTaskStatus = false;
            var regenerateSigningLinkIfExpiredTaskStatus = false;
            var regenerateDirectDebitSigningLinkIfExpiredTaskStatus = false;
            var fulfilledTaskStatus = false;
            var directDebitTaskStatus = false;
            var directDebitFollowUpTaskStatus = false;

            log.LogInformation("Running async semaphore tasks");

            try
            {
                var proposalTask = SendProposalEmailAsync(log);
                var proposalFollowUpTask = SendProposalEmailFollowUpAsync(log);
                var sendInvitationToCustomerArTask = SendInvitationToCustomerArAsync(log);
                var sendInvitationToCustomerEmployeeTask = SendInvitationToCustomerEmployeeAsync(log);
                var sendInvitationToProviderTask = SendInvitationToProviderAsync(log);
                var sendInvitationToIntroducerTask = SendInvitationToIntroducerAsync(log);
                var regenerateSigningLinkIfExpiredTask = RegenerateSigningLinkIfExpiredAsync(log);
                var regenerateDirectDebitSigningLinkIfExpiredTask = RegenerateDirectDebitSigningLinkIfExpiredAsync(log);
                var fulfilledStatusTask = CheckFulfilledStatusFromDocumentAsync(log);

                await Task.WhenAll(proposalTask, proposalFollowUpTask, sendInvitationToCustomerArTask,
                    fulfilledStatusTask, sendInvitationToProviderTask, sendInvitationToIntroducerTask,
                    regenerateSigningLinkIfExpiredTask, regenerateDirectDebitSigningLinkIfExpiredTask);

                proposalTaskStatus = await proposalTask;
                proposalFollowUpTaskStatus = await proposalFollowUpTask;
                sendInvitationToCustomerArTaskStatus = await sendInvitationToCustomerArTask;
                sendInvitationToCustomerEmployeeTaskStatus = await sendInvitationToCustomerEmployeeTask;
                sendInvitationToCustomerProviderTaskStatus = await sendInvitationToProviderTask;
                sendInvitationToCustomerIntroducerTaskStatus = await sendInvitationToIntroducerTask;
                fulfilledTaskStatus = await fulfilledStatusTask;
                regenerateSigningLinkIfExpiredTaskStatus = await regenerateSigningLinkIfExpiredTask;
                regenerateDirectDebitSigningLinkIfExpiredTaskStatus =
                    await regenerateDirectDebitSigningLinkIfExpiredTask;

                var directDebitTask = SendDirectDebitEmailAsync(log);
                var directDebitFollowUpTask = SendDirectDebitEmailFollowupAsync(log);

                await Task.WhenAll(directDebitTask, directDebitFollowUpTask);

                directDebitTaskStatus = await directDebitTask;
                directDebitFollowUpTaskStatus = await directDebitFollowUpTask;
            }
            finally
            {
                await Semaphore.WaitAsync();
                _isAllTasksCompleted = proposalTaskStatus && proposalFollowUpTaskStatus &&
                                       sendInvitationToCustomerArTaskStatus &&
                                       sendInvitationToCustomerEmployeeTaskStatus &&
                                       sendInvitationToCustomerProviderTaskStatus &&
                                       sendInvitationToCustomerIntroducerTaskStatus &&
                                       fulfilledTaskStatus && regenerateSigningLinkIfExpiredTaskStatus &&
                                       regenerateDirectDebitSigningLinkIfExpiredTaskStatus && directDebitTaskStatus &&
                                       directDebitFollowUpTaskStatus;
                Semaphore.Release();
            }
        }

        //this proposal is for both proposal and consultancy agreement combined

        #region Proposal and Consultancy Sending and Followups

        private async Task<bool> SendProposalEmailAsync(ILogger log)
        {
            log.LogInformation("Getting recipients for 'Proposal' Email...");

            try
            {
                var customersForProposal = await _customerRepository.GetCustomersForProposalEmailAsync();
                var recipientsOfProposalEmail = customersForProposal.ToList();

                if (!recipientsOfProposalEmail.Any())
                {
                    log.LogWarning("No recipients for 'Proposal' Email found");
                    return true;
                }

                var customerUpdateTask = new List<Task<Customer>>();

                foreach (var customer in recipientsOfProposalEmail)
                {
                    SetChangeInfo(customer);
                    customer.IsInProgressProposal = true;
                    customerUpdateTask.Add(_customerRepository.SaveCustomerAsync(customer));
                }

                await Task.WhenAll(customerUpdateTask);

                var consultancyEmail = Environment.GetEnvironmentVariable("Email", EnvironmentVariableTarget.Process);

                if (string.IsNullOrEmpty(consultancyEmail))
                {
                    throw new NullReferenceException(
                        $"The username should have value in {nameof(SendProposalEmailAsync)}");
                }

                var isAuthenticated = await _signatureService.AuthenticateAsync(consultancyEmail, Password);

                if (!isAuthenticated)
                {
                    throw new UnauthorizedAccessException(
                        $"{nameof(SendProposalEmailAsync)}: Not authorized to use Sign now.");
                }

                foreach (var recipient in recipientsOfProposalEmail)
                {
                    if (string.IsNullOrEmpty(recipient.Email))
                    {
                        throw new NullReferenceException($"{nameof(recipient.Email)} should not be null.");
                    }

                    SetChangeInfo(recipient);

                    var storageBaseUrl =
                        Environment.GetEnvironmentVariable("AzureStorageBaseUrl", EnvironmentVariableTarget.Process);

                    if (string.IsNullOrEmpty(storageBaseUrl))
                    {
                        throw new NullReferenceException(
                            $"{nameof(storageBaseUrl)} should not be null in {nameof(JobSignUp)}.");
                    }

                    var pdfFileLocation = $"ConversionProcess/pdf.files/{recipient.Email}/{DocumentNames.Proposal}.pdf";
                    var pdfFullUrlPath =
                        $"{storageBaseUrl}{_containerClient.Name}/{pdfFileLocation}";
                    var blobClient = _containerClient.GetBlobClient(pdfFileLocation);

                    if (!await blobClient.ExistsAsync())
                    {
                        continue;
                    }

                    var companyName = string.IsNullOrEmpty(recipient.CompanyName)
                        ? $"{recipient.FirstName} {recipient.LastName}"
                        : recipient.CompanyName;
                    var isLongFirmName = companyName.RemovePostCodeString().Length > 23;
                    var embeddedSigning =
                        await _signatureService.SendDocSignInvite(recipient, pdfFullUrlPath, isLongFirmName);
                    log.LogWarning("\'Proposal\' email sent to {RecipientEmail}", recipient.Email);

                    // Send email
                    await SendEmailConfirmationEmailAsync(recipient);

                    recipient.IsProposalEmailSent = true;

                    if (embeddedSigning != null)
                    {
                        recipient.EmbeddedSigning = embeddedSigning;
                    }

                    recipient.IsInProgressProposal = false;
                    await _customerRepository.SaveCustomerAsync(recipient);
                }

                log.LogInformation("Ended 'Proposal' Email");
            }
            catch (SignNowAuthException ex)
            {
                log.LogError("Error occured at {SendProposalEmailAsyncName}: {ExMessage}",
                    nameof(SendProposalEmailAsync), ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                log.LogError("Error occured at {SendProposalEmailAsyncName}: {ExMessage}",
                    nameof(SendProposalEmailAsync), ex.Message);
            }
            catch (Exception ex)
            {
                log.LogError("Error occured at {SendProposalEmailAsyncName}: {ExMessage}",
                    nameof(SendProposalEmailAsync), ex.Message);
            }

            return true;
        }

        private async Task<bool> SendProposalEmailFollowUpAsync(ILogger log)
        {
            log.LogInformation("Starting proposal follow up");

            try
            {
                var customersForProposal = await _customerRepository.GetCustomersNotSignedForProposalEmailAsync();
                var recipientsOfProposalEmail = customersForProposal.ToList();

                if (recipientsOfProposalEmail.Any())
                {
                    var customerUpdateTasks = new List<Task<Customer>>();

                    foreach (var customer in recipientsOfProposalEmail)
                    {
                        SetChangeInfo(customer);

                        customer.IsInProgressProposalFollowup = true;
                        customerUpdateTasks.Add(_customerRepository.SaveCustomerAsync(customer));
                    }

                    await Task.WhenAll(customerUpdateTasks);
                }

                foreach (var recipient in recipientsOfProposalEmail)
                {
                    var fourteenDays =
                        Environment.GetEnvironmentVariable("FourteenDaysInSecs", EnvironmentVariableTarget.Process);
                    var sevenDays =
                        Environment.GetEnvironmentVariable("SevenDaysInSecs", EnvironmentVariableTarget.Process);
                    var twoDays =
                        Environment.GetEnvironmentVariable("TwoDaysInSecs", EnvironmentVariableTarget.Process);
                    var oneHour =
                        Environment.GetEnvironmentVariable("OneHourInSecs", EnvironmentVariableTarget.Process);
                    var tenMinutes =
                        Environment.GetEnvironmentVariable("TenMinutesInSecs", EnvironmentVariableTarget.Process);

                    var fourteenDaysInSecs = 1209600;
                    var sevenDaysInSecs = 604800;
                    var twoDaysInSecs = 172800;
                    var oneHourInSecs = 3600;
                    var tenMinutesInSecs = 600;

                    if (int.TryParse(fourteenDays, out var result1))
                    {
                        fourteenDaysInSecs = result1;
                    }

                    if (int.TryParse(sevenDays, out var result2))
                    {
                        sevenDaysInSecs = result2;
                    }

                    if (int.TryParse(twoDays, out var result3))
                    {
                        twoDaysInSecs = result3;
                    }

                    if (int.TryParse(oneHour, out var result4))
                    {
                        oneHourInSecs = result4;
                    }

                    if (int.TryParse(tenMinutes, out var result5))
                    {
                        tenMinutesInSecs = result5;
                    }

                    SetChangeInfo(recipient);
                    var dateSignedUp = recipient.DateCreated;
                    var currentTime = DateHelper.GetCurrentDateTimeInEpoch();
                    var lapseTime = currentTime - dateSignedUp; //e.g. 1688535964 - 1688535866 = 98

                    if (lapseTime >= fourteenDaysInSecs)
                    {
                        if (recipient.IsProposal14DaysRuleSent)
                        {
                            continue;
                        }

                        log.LogInformation("14 days proposal follow up");
                        await FollowUpFourteenDaysRule(recipient);
                        continue;
                    }

                    if (lapseTime >= sevenDaysInSecs)
                    {
                        if (recipient.IsProposal7DaysRuleSent)
                        {
                            continue;
                        }

                        log.LogInformation("7 days proposal follow up");
                        await FollowUpSevenDaysRule(recipient);
                        continue;
                    }

                    if (lapseTime >= twoDaysInSecs)
                    {
                        if (recipient.IsProposal2DaysRuleSent)
                        {
                            continue;
                        }

                        log.LogInformation("2 days proposal follow up");
                        await FollowUpTwoDaysRule(recipient);
                        continue;
                    }

                    if (lapseTime >= oneHourInSecs)
                    {
                        if (recipient.IsProposal1HourRuleSent || !recipient.IsProposalDocumentViewed)
                        {
                            continue;
                        }

                        log.LogInformation("1 hour proposal follow up");
                        await FollowUpOneHourRule(recipient);
                        continue;
                    }

                    if (lapseTime < tenMinutesInSecs)
                    {
                        recipient.IsInProgressProposalFollowup = false;
                        await _customerRepository.SaveCustomerAsync(recipient);
                        log.LogWarning("The date create is not 10 mins already");
                        continue;
                    }

                    if (recipient.IsProposalDocumentViewed || recipient.IsProposal10MinsRuleSent)
                    {
                        recipient.IsInProgressProposalFollowup = false;
                        await _customerRepository.SaveCustomerAsync(recipient);
                        log.LogWarning("The 10 mins follow up is sent alrady or doc has viewed");
                        continue;
                    }

                    log.LogInformation("10 mins proposal follow up");
                    await FollowUpTenMinutesRule(recipient);
                }

                log.LogInformation("Ended proposal follow up");
            }
            catch (Exception ex)
            {
                log.LogError("Error occured at {SendProposalEmailFollowUpAsyncName}: {ExMessage}",
                    nameof(SendProposalEmailFollowUpAsync), ex.Message);
            }

            return true;
        }

        private async Task<bool> SendInvitationToCustomerArAsync(ILogger log)
        {
            log.LogInformation("Getting recipients for 'Invitation to Customer AR' Email...");

            try
            {
                var customerAppointedRepresentatives =
                    await _appointedRepresentativeRepository.GetAppointedRepresentativesAsync();
                var recipientsInvitationToCustomerArEmail = customerAppointedRepresentatives.ToList();

                if (!recipientsInvitationToCustomerArEmail.Any())
                {
                    log.LogWarning("No recipients for 'Invitation to Customer AR' Email found");
                    return true;
                }

                foreach (var recipient in recipientsInvitationToCustomerArEmail.Where(recipient =>
                             !string.IsNullOrEmpty(recipient.Email)))
                {
                    const string htmlFileName = "send_invitation_to_customer_ar.html";
                    const string subFolder = "AppointedRepresentative";
                    var htmlContent = await GetHtmlFullUrlPathFromStorageAsync(htmlFileName, subFolder);

                    var baseUrl =
                        Environment.GetEnvironmentVariable("BaseRedirectUrl", EnvironmentVariableTarget.Process);
                    var invitationUrl = $"{baseUrl}/ar-verify/{Helpers.EncodeToBase64(recipient.Email)}";
                    var emailAddress = recipient.Email.Split('@')[0];

                    htmlContent = htmlContent.Replace("$(EmailAddress)", emailAddress);
                    htmlContent = htmlContent.Replace("$(InvitationUrl)", invitationUrl);

                    if (string.IsNullOrEmpty(recipient.Email))
                    {
                        throw new NullReferenceException(
                            $"The recipient email is required in the front end. Why it is null in DB?");
                    }

                    const string subject = "Invitation to Customer AR";
                    _emailService.SendEmail(recipient.Email, subject, htmlContent);

                    recipient.IsFinishedSignUp = true;
                    await _appointedRepresentativeRepository.SaveOrUpdateAppointedRepresentativeAsync(recipient);
                }
            }
            catch (Exception ex)
            {
                log.LogError("Error occured at {SendInvitationToCustomerArAsyncName}: {ExMessage}",
                    nameof(SendInvitationToCustomerArAsync), ex.Message);
            }

            return true;
        }

        private async Task<bool> SendInvitationToCustomerEmployeeAsync(ILogger log)
        {
            log.LogInformation("Getting recipients for 'Invitation to Customer Employee' Email...");

            try
            {
                var completeEmployeeDetailsRequestRecipients =
                    (await _organizationalStructureRepository.GetAllEmployeesNotYetFinishedSignupAsync()).ToList();

                if (!completeEmployeeDetailsRequestRecipients.Any())
                {
                    log.LogWarning("No recipients for 'Invitation to Customer Employee' Email found");
                    return true;
                }

                foreach (var recipient in completeEmployeeDetailsRequestRecipients.Where(recipient =>
                             !string.IsNullOrEmpty(recipient.Email) &&
                             recipient.ProfileStatus == ProfileStatuses.Full.ToString()))
                {
                    const string htmlFileName = "send_invitation_to_customer_employee.html";
                    const string subFolder = "CustomerEmployee";
                    var htmlContent = await GetHtmlFullUrlPathFromStorageAsync(htmlFileName, subFolder);

                    var baseUrl =
                        Environment.GetEnvironmentVariable("BaseRedirectUrl", EnvironmentVariableTarget.Process);
                    var invitationUrl = $"{baseUrl}/employee-verify/{Helpers.EncodeToBase64(recipient.Email)}";
                    var emailAddress = recipient.Email.Split('@')[0];

                    htmlContent = htmlContent.Replace("$(EmailAddress)", emailAddress);
                    htmlContent = htmlContent.Replace("$(InvitationUrl)", invitationUrl);

                    if (string.IsNullOrEmpty(recipient.Email))
                    {
                        throw new NullReferenceException(
                            $"The recipient email is required in the front end. Why it is null in DB?");
                    }

                    const string subject = "Invitation to Employee";
                    _emailService.SendEmail(recipient.Email, subject, htmlContent);

                    recipient.IsFinishedSignUp = true;
                    await _organizationalStructureRepository.SaveOrUpdateEmployeeAsync(recipient);
                }
            }
            catch (Exception ex)
            {
                log.LogError("Error occured at {SendInvitationToCustomerEmployeeAsyncName}: {ExMessage}",
                    nameof(SendInvitationToCustomerEmployeeAsync), ex.Message);
            }

            return true;
        }

        private async Task<bool> SendInvitationToProviderAsync(ILogger log)
        {
            log.LogInformation("Getting recipients for 'Invitation to Provider' Email...");

            try
            {
                var completeProviderDetailsRequestRecipients =
                    (await _providersRepository.GetAllProvidersNotYetFinishedSignupAsync()).ToList();

                if (!completeProviderDetailsRequestRecipients.Any())
                {
                    log.LogWarning("No recipients for 'Invitation to Customer Provider' Email found");
                    return true;
                }

                foreach (var recipient in completeProviderDetailsRequestRecipients.Where(recipient =>
                             !string.IsNullOrEmpty(recipient.Email) &&
                             recipient.ProfileStatus == ProfileStatuses.Full.ToString()))
                {
                    const string htmlFileName = "send_invitation_to_customer_provider.html";
                    const string subFolder = "CustomerProvider";
                    var htmlContent = await GetHtmlFullUrlPathFromStorageAsync(htmlFileName, subFolder);

                    var baseUrl =
                        Environment.GetEnvironmentVariable("BaseRedirectUrl", EnvironmentVariableTarget.Process);
                    var invitationUrl = $"{baseUrl}/provider-verify/{Helpers.EncodeToBase64(recipient.Email)}";
                    var emailAddress = recipient.Email.Split('@')[0];

                    htmlContent = htmlContent.Replace("$(EmailAddress)", emailAddress);
                    htmlContent = htmlContent.Replace("$(InvitationUrl)", invitationUrl);

                    if (string.IsNullOrEmpty(recipient.Email))
                    {
                        throw new NullReferenceException(
                            $"The recipient email is required in the front end. Why it is null in DB?");
                    }

                    const string subject = "Invitation to Provider";
                    _emailService.SendEmail(recipient.Email, subject, htmlContent);

                    recipient.IsFinishedSignUp = true;
                    await _providersRepository.SaveOrUpdateProvidersAsync(recipient);
                }
            }
            catch (Exception ex)
            {
                log.LogError("Error occured at {SendInvitationToProviderAsyncName}: {ExMessage}",
                    nameof(SendInvitationToProviderAsync), ex.Message);
            }

            return true;
        }

        private async Task<bool> SendInvitationToIntroducerAsync(ILogger log)
        {
            log.LogInformation("Getting recipients for 'Invitation to Introducer' Email...");

            try
            {
                var completeIntroducerDetailsRequestRecipients =
                    (await _introducersRepository.GetAllIntroducersNotYetFinishedSignupAsync()).ToList();

                if (!completeIntroducerDetailsRequestRecipients.Any())
                {
                    log.LogWarning("No recipients for 'Invitation to Customer Introducer' Email found");
                    return true;
                }

                foreach (var recipient in completeIntroducerDetailsRequestRecipients.Where(recipient =>
                             !string.IsNullOrEmpty(recipient.Email) &&
                             recipient.ProfileStatus == ProfileStatuses.Full.ToString()))
                {
                    const string htmlFileName = "send_invitation_to_customer_introducer.html";
                    const string subFolder = "CustomerIntroducer";
                    var htmlContent = await GetHtmlFullUrlPathFromStorageAsync(htmlFileName, subFolder);

                    var baseUrl =
                        Environment.GetEnvironmentVariable("BaseRedirectUrl", EnvironmentVariableTarget.Process);
                    var invitationUrl = $"{baseUrl}/introducer-verify/{Helpers.EncodeToBase64(recipient.Email)}";
                    var emailAddress = recipient.Email.Split('@')[0];

                    htmlContent = htmlContent.Replace("$(EmailAddress)", emailAddress);
                    htmlContent = htmlContent.Replace("$(InvitationUrl)", invitationUrl);

                    if (string.IsNullOrEmpty(recipient.Email))
                    {
                        throw new NullReferenceException(
                            $"The recipient email is required in the front end. Why it is null in DB?");
                    }

                    const string subject = "Invitation to Introducer";
                    _emailService.SendEmail(recipient.Email, subject, htmlContent);

                    recipient.IsFinishedSignUp = true;
                    await _introducersRepository.SaveOrUpdateIntroducersAsync(recipient);
                }
            }
            catch (Exception ex)
            {
                log.LogError("Error occured at {SendInvitationToIntroducerAsyncName}: {ExMessage}",
                    nameof(SendInvitationToIntroducerAsync), ex.Message);
            }

            return true;
        }

        private async Task<bool> RegenerateSigningLinkIfExpiredAsync(ILogger log)
        {
            log.LogInformation("Starting regeneration of expired links");

            try
            {
                var customerNotSignedProposal = await _customerRepository.GetCustomersNotSignedForProposalEmailAsync();

                if (customerNotSignedProposal.Any())
                {
                    var customerUpdateTasks = new List<Task<Customer>>();

                    foreach (var customer in customerNotSignedProposal)
                    {
                        SetChangeInfo(customer);
                        customer.IsGeneratingSigningLink = true;
                        customerUpdateTasks.Add(_customerRepository.SaveCustomerAsync(customer));
                    }

                    await Task.WhenAll(customerUpdateTasks);
                }

                var username = Environment.GetEnvironmentVariable("Email", EnvironmentVariableTarget.Process);

                if (string.IsNullOrEmpty(username))
                {
                    throw new NullReferenceException(
                        $"The username should have value in {nameof(RegenerateSigningLinkIfExpiredAsync)}");
                }

                var isAuthenticated = await _signatureService.AuthenticateAsync(username, Password);

                if (!isAuthenticated)
                {
                    throw new UnauthorizedAccessException(
                        $"{nameof(RegenerateSigningLinkIfExpiredAsync)}: Not authorized to use Sign now.");
                }

                foreach (var customer in customerNotSignedProposal)
                {
                    SetChangeInfo(customer);
                    if (customer.EmbeddedSigning?.Expiry == null)
                    {
                        continue;
                    }

                    if (DateHelper.ConvertDateTimeToEpoch(DateTime.UtcNow) < customer.EmbeddedSigning.Expiry.Value)
                    {
                        continue;
                    }

                    log.LogWarning("Signing link for {CustomerEmail} is expired. Recreating new link...",
                        customer.Email);
                    var docId = customer.EmbeddedSigning.DocumentId ?? throw new NullReferenceException(
                        $"doc Id should not be null in db when call in {nameof(RegenerateSigningLinkIfExpiredAsync)}");
                    var fieldInviteId = customer.EmbeddedSigning.FieldInviteId ??
                                        throw new NullReferenceException(
                                            $"fieldinvite Id should not be null in db when call in {nameof(RegenerateSigningLinkIfExpiredAsync)}");
                    var embeddedSigning = await _signatureService.CreateEmbeddedInviteLinkAsync(docId, fieldInviteId);

                    var created = customer.EmbeddedSigning.Created;
                    customer.EmbeddedSigning = embeddedSigning;
                    customer.EmbeddedSigning.Created = created;
                    customer.IsGeneratingSigningLink = false;
                    await _customerRepository.SaveCustomerAsync(customer);
                }

                log.LogInformation("Regeneration of expired links done");
            }
            catch (SignNowAuthException ex)
            {
                log.LogError("Error occured at {SendProposalEmailAsyncName}: {ExMessage}",
                    nameof(SendProposalEmailAsync), ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                log.LogError("Error occured at {SendProposalEmailAsyncName}: {ExMessage}",
                    nameof(SendProposalEmailAsync), ex.Message);
            }

            return true;
        }

        public async Task<bool> RegenerateDirectDebitSigningLinkIfExpiredAsync(ILogger log)
        {
            log.LogInformation("Starting regeneration of expired direct debit links");

            try
            {
                var customerNotSignedDirectDebit = await _customerRepository.GetCustomersNotSignedDirectDebitAsync();
                var username = Environment.GetEnvironmentVariable("Email", EnvironmentVariableTarget.Process);

                if (customerNotSignedDirectDebit.Any())
                {
                    var customerUpdateTasks = new List<Task<Customer>>();

                    foreach (var customer in customerNotSignedDirectDebit)
                    {
                        SetChangeInfo(customer);
                        customer.IsGeneratingDirectDebitSigningLink = true;
                        customerUpdateTasks.Add(_customerRepository.SaveCustomerAsync(customer));
                    }

                    await Task.WhenAll(customerUpdateTasks);
                }

                if (string.IsNullOrEmpty(username))
                {
                    throw new NullReferenceException(
                        $"The username should have value in {nameof(RegenerateDirectDebitSigningLinkIfExpiredAsync)}");
                }

                var isAuthenticated = await _signatureService.AuthenticateAsync(username, Password);

                if (!isAuthenticated)
                {
                    throw new UnauthorizedAccessException(
                        $"{nameof(RegenerateDirectDebitSigningLinkIfExpiredAsync)}: Not authorized to use Sign now.");
                }

                foreach (var customer in customerNotSignedDirectDebit)
                {
                    SetChangeInfo(customer);
                    if (customer.EmbeddedDirectDebitSigning?.Expiry == null)
                    {
                        continue;
                    }

                    if (DateHelper.ConvertDateTimeToEpoch(DateTime.UtcNow) <
                        customer.EmbeddedDirectDebitSigning.Expiry.Value)
                    {
                        continue;
                    }

                    log.LogWarning("Direct Debit Signing link for {CustomerEmail} is expired. Recreating new link...",
                        customer.Email);

                    var docId = customer.EmbeddedDirectDebitSigning.DocumentId ?? throw new NullReferenceException(
                        $"doc Id should not be null in db when call in {nameof(RegenerateDirectDebitSigningLinkIfExpiredAsync)}");
                    var fieldInviteId = customer.EmbeddedDirectDebitSigning.FieldInviteId ??
                                        throw new NullReferenceException(
                                            $"fieldinvite Id should not be null in db when call in {nameof(RegenerateDirectDebitSigningLinkIfExpiredAsync)}");
                    var embeddedSigning = await _signatureService.CreateEmbeddedInviteLinkAsync(docId, fieldInviteId);

                    var created = customer.EmbeddedDirectDebitSigning.Created;
                    customer.EmbeddedDirectDebitSigning = embeddedSigning;
                    customer.EmbeddedDirectDebitSigning.Created = created;
                    customer.IsGeneratingDirectDebitSigningLink = false;
                    await _customerRepository.SaveCustomerAsync(customer);
                }

                log.LogInformation("Regeneration of expired direct debit links done");
            }
            catch (SignNowAuthException ex)
            {
                log.LogError("Error occured at {SendProposalEmailAsyncName}: {ExMessage}",
                    nameof(SendProposalEmailAsync), ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                log.LogError("Error occured at {SendProposalEmailAsyncName}: {ExMessage}",
                    nameof(SendProposalEmailAsync), ex.Message);
            }

            return true;
        }

        private async Task FollowUpFourteenDaysRule(Customer recipient)
        {
            const string htmlFileName = "follow.up.email.proposal.5.html";
            var htmlContent = await GetHtmlFullUrlPathFromStorageAsync(htmlFileName);
            var consultancyName = await GetConsultancyNameAsync();
            var consultancyMobileNumber = await GetConsultancyMobileNumberAsync();
            var fcaRegulationUrl = await GetFcaRegulationUrlAsync();
            var firmName = $"{recipient.FirstName} {recipient.LastName}";
            var expirationDate = DateTime.Now.AddMonths(1).ToString($"{AppConstants.DateFormatDefault} hh:mm:ss tt");
            var viewProposalUrl = GetViewProposalUrl(recipient);

            htmlContent = htmlContent.Replace("$(ConsultancyName)", consultancyName);
            htmlContent = htmlContent.Replace("$(RepresentativeName)", firmName);
            htmlContent = htmlContent.Replace("$(FCARegulationUrl)", fcaRegulationUrl);
            htmlContent = htmlContent.Replace("$(ViewProposalUrl)", viewProposalUrl);
            htmlContent = htmlContent.Replace("$(Date)", expirationDate);
            htmlContent = htmlContent.Replace("$(ConsultancyMobileNumber)", consultancyMobileNumber);
            htmlContent = htmlContent.Replace("$(PlatformName)", consultancyName);

            if (string.IsNullOrEmpty(recipient.Email))
            {
                throw new NullReferenceException(
                    $"The recipient email is required in the front end. Why it is null in DB?");
            }

            var subject = $"{consultancyName}'s Proposal";
            _emailService.SendEmail(recipient.Email, subject, htmlContent);
            recipient.IsProposal14DaysRuleSent = true;
            recipient.IsInProgressProposalFollowup = false;
            await _customerRepository.SaveCustomerAsync(recipient);
        }

        private async Task SendEmailConfirmationEmailAsync(Customer recipient)
        {
            const string htmlFileName = "email.confirmation.html";
            var htmlContent = await GetHtmlFullUrlPathFromStorageAsync(htmlFileName);
            var consultancyName = await GetConsultancyNameAsync();
            var viewProposalUrl = GetViewProposalPortalUrl(recipient);

            htmlContent = htmlContent.Replace("$(PlatformName)", consultancyName);
            htmlContent = htmlContent.Replace("$(Forename(s))", recipient.FirstName);
            htmlContent = htmlContent.Replace("$(ViewProposalUrl)", viewProposalUrl);

            if (string.IsNullOrEmpty(recipient.Email))
            {
                throw new NullReferenceException(
                    $"The recipient email is required in the front end. Why it is null in DB?");
            }

            var subject = $"{consultancyName}'s Proposal";
            _emailService.SendEmail(recipient.Email, subject, htmlContent);
        }

        // ToDo. the calendarUrl, date and time variable are doesn't have data yet
        private async Task FollowUpSevenDaysRule(Customer recipient)
        {
            const string htmlFileName = "follow.up.email.proposal.4.html";
            var htmlContent = await GetHtmlFullUrlPathFromStorageAsync(htmlFileName);
            var consultancyName = await GetConsultancyNameAsync();
            var consultancyMobileNumber = await GetConsultancyMobileNumberAsync();
            var fcaRegulationUrl = await GetFcaRegulationUrlAsync();
            var firmName = $"{recipient.FirstName} {recipient.LastName}";
            var viewProposalUrl = GetViewProposalUrl(recipient);

            htmlContent = htmlContent.Replace("$(ConsultancyName)", consultancyName);
            htmlContent = htmlContent.Replace("$(RepresentativeName)", firmName);
            htmlContent = htmlContent.Replace("$(ViewProposalUrl)", viewProposalUrl);
            htmlContent = htmlContent.Replace("$(FCARegulationUrl)", fcaRegulationUrl);
            htmlContent = htmlContent.Replace("$(ConsultancyMobileNumber)", consultancyMobileNumber);
            var date = DateTime.UtcNow.Date.AddDays(1).ToString(AppConstants.DateFormatDefault);
            const string time = "09:00 AM"; //ToDo. right now we assume 9AM consultant will call
            htmlContent = htmlContent.Replace("$(Date)", date);
            htmlContent = htmlContent.Replace("$(Time)", time);
            htmlContent = htmlContent.Replace("$(PlatformName)", consultancyName);

            //add scheduling here the first one
            var applicationUserAccount = await _calendlyService.GetApplicationAccountAsync();
            if (!string.IsNullOrEmpty(applicationUserAccount?.Resource?.UserUrl))
            {
                var eventTypes = await _calendlyService.GetEventTypesAsync(applicationUserAccount.Resource.UserUrl);
                var calendarUrl = eventTypes?.Collection.FirstOrDefault()?.SchedulingUrl ?? "";
                htmlContent = htmlContent.Replace("$(CalendarUrl)", calendarUrl);
            }

            if (string.IsNullOrEmpty(recipient.Email))
            {
                throw new NullReferenceException(
                    $"The recipient email is required in the front end. Why it is null in DB?");
            }

            var subject = $"{consultancyName}'s Proposal";
            _emailService.SendEmail(recipient.Email, subject, htmlContent);
            recipient.IsProposal7DaysRuleSent = true;
            recipient.IsInProgressProposalFollowup = false;
            await _customerRepository.SaveCustomerAsync(recipient);
        }

        private async Task FollowUpTwoDaysRule(Customer recipient)
        {
            const string htmlFileName = "follow.up.email.proposal.2.html";

            var htmlContent = await GetHtmlFullUrlPathFromStorageAsync(htmlFileName);
            var consultancyName = await GetConsultancyNameAsync();
            var consultancyMobileNumber = await GetConsultancyMobileNumberAsync();
            var companyName = GetCompanyName(recipient);
            var firmName = $"{recipient.FirstName} {recipient.LastName}";
            var viewProposalUrl = GetViewProposalUrl(recipient);

            htmlContent = htmlContent.Replace("$(ConsultancyName)", consultancyName);
            htmlContent = htmlContent.Replace("$(RepresentativeName)", firmName);
            htmlContent = htmlContent.Replace("$(ViewProposalUrl)", viewProposalUrl);
            htmlContent = htmlContent.Replace("$(FirmName)",
                recipient.IsCompanyNotApplicable ? firmName : companyName);
            htmlContent = htmlContent.Replace("$(ConsultancyMobileNumber)", consultancyMobileNumber);
            htmlContent = htmlContent.Replace("$(PlatformName)", consultancyName);

            if (string.IsNullOrEmpty(recipient.Email))
            {
                throw new NullReferenceException(
                    $"The recipient email is required in the front end. Why it is null in DB?");
            }

            var subject = $"{consultancyName}'s Proposal";
            _emailService.SendEmail(recipient.Email, subject, htmlContent);
            recipient.IsProposal2DaysRuleSent = true;
            recipient.IsInProgressProposalFollowup = false;
            await _customerRepository.SaveCustomerAsync(recipient);
        }

        private async Task FollowUpOneHourRule(Customer recipient)
        {
            await ComposeAndSendTenMinuteOrOneHourRuleEmailAsync(recipient);
            recipient.IsProposal1HourRuleSent = true;
            recipient.IsInProgressProposalFollowup = false;
            await _customerRepository.SaveCustomerAsync(recipient);
        }

        private async Task FollowUpTenMinutesRule(Customer recipient)
        {
            await ComposeAndSendTenMinuteOrOneHourRuleEmailAsync(recipient);
            recipient.IsProposal10MinsRuleSent = true;
            recipient.IsInProgressProposalFollowup = false;
            await _customerRepository.SaveCustomerAsync(recipient);
        }

        private async Task ComposeAndSendTenMinuteOrOneHourRuleEmailAsync(Customer recipient)
        {
            const string htmlFileName = "follow.up.email.proposal.1.html";
            var htmlContent = await GetHtmlFullUrlPathFromStorageAsync(htmlFileName);
            var consultancyName = await GetConsultancyNameAsync();
            var consultancyMobileNumber = await GetConsultancyMobileNumberAsync();
            var companyName = GetCompanyName(recipient);
            var firmName = $"{recipient.FirstName} {recipient.LastName}";
            var viewProposalUrl = GetViewProposalUrl(recipient);

            htmlContent = htmlContent.Replace("$(ConsultancyName)", consultancyName);
            htmlContent = htmlContent.Replace("$(RepresentativeName)", firmName);
            htmlContent = htmlContent.Replace("$(ViewProposalUrl)", viewProposalUrl);
            htmlContent = htmlContent.Replace("$(FirmName)",
                recipient.IsCompanyNotApplicable ? firmName : companyName);
            htmlContent = htmlContent.Replace("$(ConsultancyMobileNumber)", consultancyMobileNumber);
            htmlContent = htmlContent.Replace("$(PlatformName)", consultancyName);

            if (string.IsNullOrEmpty(recipient.Email))
            {
                throw new NullReferenceException(
                    $"The recipient email is required in the front end. Why it is null in DB?");
            }

            var subject = $"{consultancyName}'s Proposal";
            _emailService.SendEmail(recipient.Email, subject, htmlContent);
        }

        private static string GetViewProposalUrl(Customer recipient)
        {
            var queryStringBase64 = Helpers.EncodeToBase64($"email={recipient.Email}");
            return $"{Environment.GetEnvironmentVariable("BaseRedirectUrl", EnvironmentVariableTarget.Process)}" +
                   $"/doc-sign-view?{queryStringBase64}";
        }

        private static string GetViewProposalPortalUrl(Customer recipient)
        {
            var queryStringBase64 = Helpers.EncodeToBase64($"email={recipient.Email}");
            return $"{Environment.GetEnvironmentVariable("BaseRedirectUrl", EnvironmentVariableTarget.Process)}" +
                   $"/portal?{queryStringBase64}";
        }

        private async Task<string> GetConsultancyNameAsync()
        {
            var consultancyName = (await _settingRepository.GetAllSettingAsync())
                .FirstOrDefault(s => s.Key == "$(CONSULTANCY_NAME)")?.Value;

            if (string.IsNullOrEmpty(consultancyName))
            {
                throw new NullReferenceException(
                    $"The consultancy name must have a value in {nameof(GetConsultancyNameAsync)}");
            }

            return consultancyName;
        }

        private async Task<string> GetFcaRegulationUrlAsync()
        {
            var fcaRegulationUrl = (await _settingRepository.GetAllSettingAsync())
                .FirstOrDefault(s => s.Key == "$(FCARegulationUrl)")
                ?.Value;

            if (string.IsNullOrEmpty(fcaRegulationUrl))
            {
                throw new NullReferenceException(
                    $"The FCA Regulation Url have a value in {nameof(GetFcaRegulationUrlAsync)}");
            }

            return fcaRegulationUrl;
        }

        private async Task<string> GetConsultancyMobileNumberAsync()
        {
            var consultancyMobileNumber = (await _settingRepository.GetAllSettingAsync())
                .FirstOrDefault(s => s.Key == "$(CONSULTANCY_MOBILE_NO)")
                ?.Value;

            if (string.IsNullOrEmpty(consultancyMobileNumber))
            {
                throw new NullReferenceException(
                    $"The Consultancy Mobile Number must have a value in {nameof(GetConsultancyMobileNumberAsync)}");
            }

            return consultancyMobileNumber;
        }

        private async Task<string> GetHtmlFullUrlPathFromStorageAsync(string htmlFileName,
            string subFolder = "ProposalConsultancyAgreement")
        {
            var blobStorageBaseUrl =
                Environment.GetEnvironmentVariable("AzureStorageBaseUrl", EnvironmentVariableTarget.Process);
            var blobContainerName =
                Environment.GetEnvironmentVariable("BlobStorageContainerName", EnvironmentVariableTarget.Process);
            var htmlFileUrl =
                $"{blobStorageBaseUrl}{blobContainerName}/EmailTemplates/{subFolder}/{htmlFileName}";

            using var client = new HttpClient();
            var htmlContents = await client.GetStringAsync(htmlFileUrl);

            if (string.IsNullOrEmpty(htmlContents))
            {
                throw new NullReferenceException(
                    $"The html content should have a value in {nameof(GetHtmlFullUrlPathFromStorageAsync)}");
            }

            return htmlContents;
        }

        private static string GetCompanyName(Customer customer)
        {
            var companyName = customer.CompanyName;

            if (!string.IsNullOrEmpty(companyName) && companyName.Contains(" ("))
            {
                companyName = companyName[..companyName.IndexOf(" (", StringComparison.Ordinal)];
            }

            return companyName;
        }

        #endregion

        private async Task<bool> SendDirectDebitEmailAsync(ILogger log)
        {
            log.LogInformation("Getting recipients for 'Direct Debit' Email...");

            try
            {
                var customersForDirectDebit = await _customerRepository.GetCustomersForDirectDebitEmailAsync();
                var directDebitList = customersForDirectDebit.ToList();

                if (!directDebitList.Any())
                {
                    log.LogWarning("No recipients for 'Direct Debit' Email found");
                    return true;
                }

                var customerUpdateTasks = new List<Task<Customer>>();

                foreach (var customer in directDebitList)
                {
                    SetChangeInfo(customer);
                    customer.IsInProgressDirectDebit = true;
                    customerUpdateTasks.Add(_customerRepository.SaveCustomerAsync(customer));
                }

                await Task.WhenAll(customerUpdateTasks);

                var consultancyEmail = Environment.GetEnvironmentVariable("Email", EnvironmentVariableTarget.Process);

                if (string.IsNullOrEmpty(consultancyEmail))
                {
                    throw new NullReferenceException(
                        $"The username should have value in {nameof(SendDirectDebitEmailAsync)}");
                }

                var isAuthenticated = await _signatureService.AuthenticateAsync(consultancyEmail, Password);

                if (!isAuthenticated)
                {
                    throw new UnauthorizedAccessException(
                        $"{nameof(SendDirectDebitEmailAsync)}: Not authorized to use Sign now.");
                }

                foreach (var customer in directDebitList)
                {
                    log.LogInformation("Sending email to {CustomerEmail}...", customer.Email);

                    if (string.IsNullOrEmpty(customer.Email))
                    {
                        throw new NullReferenceException(
                            $"customer {customer.FirstName} should have an email in {nameof(SendDirectDebitEmailAsync)}");
                    }

                    SetChangeInfo(customer);

                    var storageBaseUrl =
                        Environment.GetEnvironmentVariable("AzureStorageBaseUrl", EnvironmentVariableTarget.Process);

                    if (string.IsNullOrEmpty(storageBaseUrl))
                    {
                        throw new NullReferenceException(
                            $"{nameof(storageBaseUrl)} should not be null in {nameof(JobSignUp)}.");
                    }

                    var pdfFileLocation =
                        $"ConversionProcess/raw.pdf.docs/ComplyDex/{DocumentNames.DirectDebitMandate}.pdf";
                    var pdfFullUrlPath =
                        $"{storageBaseUrl}{_containerClient.Name}/{pdfFileLocation}";
                    var blobClient = _containerClient.GetBlobClient(pdfFileLocation);

                    if (!await blobClient.ExistsAsync())
                    {
                        customer.IsInProgressDirectDebit = false;
                        await _customerRepository.SaveCustomerAsync(customer);
                        continue;
                    }

                    var embeddedSigning =
                        await _signatureService.SendDocSignInviteAndEditPdf(customer, pdfFullUrlPath);
                    log.LogWarning("\'Direct Debit Mandate\' embedded signing link generated for {CustomerEmail}",
                        customer.Email);

                    await SendDirectDebitEmailAsync(customer, embeddedSigning);
                    log.LogWarning("\'Direct Debit Email\' successfully sent to {CustomerEmail}", customer.Email);
                }

                log.LogInformation("Ended for 'Direct Debit' Email");
            }
            catch (SignNowAuthException ex)
            {
                log.LogError("Error occured at {SendProposalEmailAsyncName}: {ExMessage}",
                    nameof(SendProposalEmailAsync), ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                log.LogError("Error occured at {SendProposalEmailAsyncName}: {ExMessage}",
                    nameof(SendProposalEmailAsync), ex.Message);
            }
            catch (Exception ex)
            {
                log.LogError("Error occured at {SendDirectDebitEmailAsync}: {ExMessage}",
                    nameof(SendDirectDebitEmailAsync), ex.Message);
            }

            return true;
        }

        private async Task SendDirectDebitEmailAsync(Customer recipient, EmbeddedSigning embeddedSigningLink)
        {
            const string htmlFileName = "direct_debit_for_firm_representative.html";
            const string subject = "Direct Debit Mandate";

            var htmlContent = await GetHtmlFullUrlPathFromStorageAsync(htmlFileName, "DirectDebit");
            var consultancyName = await GetConsultancyNameAsync();
            var consultancyMobileNumber = await GetConsultancyMobileNumberAsync();
            var companyName = GetCompanyName(recipient);
            var firmName = $"{recipient.FirstName} {recipient.LastName}";
            var directDebitMandateUrl = GetViewDirectDebitMandateUrl(recipient);

            htmlContent = htmlContent.Replace("$(RepresentativeName)",
                recipient.IsCompanyNotApplicable ? firmName : companyName);
            htmlContent = htmlContent.Replace("$(ConsultancyName)", consultancyName);
            htmlContent = htmlContent.Replace("$(ConsultancyMobileNumber)", consultancyMobileNumber);
            htmlContent = htmlContent.Replace("$(DirectDebitMandateUrl)", directDebitMandateUrl);
            htmlContent = htmlContent.Replace("$(PlatformName)", consultancyName);

            if (string.IsNullOrEmpty(recipient.Email))
            {
                throw new NullReferenceException(
                    $"The recipient email is required in the front end. Why it is null in DB?");
            }

            _emailService.SendEmail(recipient.Email, subject, htmlContent);
            recipient.IsDirectDebitEmailSent = true;
            recipient.EmbeddedDirectDebitSigning = embeddedSigningLink;
            recipient.IsInProgressDirectDebit = false;
            await _customerRepository.SaveCustomerAsync(recipient);
        }

        private async Task<bool> SendDirectDebitEmailFollowupAsync(ILogger log)
        {
            log.LogInformation("Getting recipients for 'Direct Debit Follow up' Email...");

            try
            {
                var customersForDirectDebit = await _customerRepository.GetCustomersForDirectDebitFollowupAsync();
                var directDebitList = customersForDirectDebit.ToList();

                if (!directDebitList.Any())
                {
                    log.LogWarning("No recipients for 'Direct Debit Follow Up' Email found");
                    return true;
                }

                var customerUpdateTasks = new List<Task<Customer>>();

                foreach (var customer in directDebitList)
                {
                    customer.IsInProgressDirectDebitFollowup = true;
                    customerUpdateTasks.Add(_customerRepository.SaveCustomerAsync(customer));
                }

                await Task.WhenAll(customerUpdateTasks);

                foreach (var customer in directDebitList)
                {
                    SetChangeInfo(customer);
                    if (customer.EmbeddedDirectDebitSigning == null)
                    {
                        log.LogWarning(
                            $"The direct debit signing link should have value in {nameof(SendDirectDebitEmailFollowupAsync)} for customer {customer.Email}.");
                        customer.IsInProgressDirectDebitFollowup = false;
                        await _customerRepository.SaveCustomerAsync(customer);
                        continue;
                    }

                    var triggerTime =
                        Environment.GetEnvironmentVariable("DirectDebitFollowUpTriggerTimeInEpoch",
                            EnvironmentVariableTarget.Process);

                    var requiredElapsedTime = 360; // default to 5 minutes

                    if (int.TryParse(triggerTime, out var triggerTimeEpoch))
                    {
                        requiredElapsedTime = triggerTimeEpoch;
                    }

                    var dateDirectDebitSigningLinkCreated = customer.EmbeddedDirectDebitSigning.Created;
                    var currentTime = DateHelper.GetCurrentDateTimeInEpoch();
                    var lapseTime = currentTime - dateDirectDebitSigningLinkCreated;

                    if (lapseTime < requiredElapsedTime)
                    {
                        customer.IsInProgressDirectDebitFollowup = false;
                        await _customerRepository.SaveCustomerAsync(customer);
                        continue;
                    }

                    log.LogInformation("Sending follow up email to {CustomerEmail}...", customer.Email);

                    var firmRepresentativeEmail = customer.FirmRepresentativeDetail?.EmailAddress;

                    if (string.IsNullOrEmpty(firmRepresentativeEmail))
                    {
                        throw new NullReferenceException(
                            $"customer {customer.Email} should have a firm representative email in {nameof(SendDirectDebitEmailFollowupAsync)}");
                    }

                    log.LogWarning(
                        "Customer/Owner's \'Direct Debit Mandate\' embedded signing link is used for firm representative {RepresentativeEmail}",
                        firmRepresentativeEmail);

                    await SendDirectDebitEmailForFirmRepresentativeAsync(customer, customer.EmbeddedDirectDebitSigning);
                    log.LogWarning(
                        "\'Direct Debit Email\' successfully sent to Firm Representative {RepresentativeEmail}",
                        firmRepresentativeEmail);
                }
            }
            catch (Exception ex)
            {
                log.LogError("Error occured at {SendDirectDebitEmailFollowupAsync}: {ExMessage}",
                    nameof(SendDirectDebitEmailFollowupAsync), ex.Message);
            }

            return true;
        }

        private async Task SendDirectDebitEmailForFirmRepresentativeAsync(Customer recipient,
            EmbeddedSigning embeddedSigningLink)
        {
            const string htmlFileName = "direct_debit_for_account_representative.html";
            const string subject = "Direct Debit Mandate for Firm Representative";

            var htmlContent = await GetHtmlFullUrlPathFromStorageAsync(htmlFileName, "DirectDebit");
            var consultancyName = await GetConsultancyNameAsync();
            var consultancyMobileNumber = await GetConsultancyMobileNumberAsync();
            var directDebitMandateUrl = GetViewDirectDebitMandateUrl(recipient);
            var companyName = GetCompanyName(recipient);
            var ownerName = $"{recipient.FirstName} {recipient.LastName}";
            var representativeName =
                $"{recipient.FirmRepresentativeDetail?.Forename} {recipient.FirmRepresentativeDetail?.Surname}";

            htmlContent = htmlContent.Replace("$(AccountRepresentativeName)", representativeName);
            htmlContent =
                htmlContent.Replace("$(FirmName)", recipient.IsCompanyNotApplicable ? ownerName : companyName);
            htmlContent = htmlContent.Replace("$(ConsultancyName)", consultancyName);
            htmlContent = htmlContent.Replace("$(ConsultancyMobileNumber)", consultancyMobileNumber);
            htmlContent = htmlContent.Replace("$(DirectDebitMandateUrl)", directDebitMandateUrl);
            htmlContent = htmlContent.Replace("$(PlatformName)", consultancyName);

            var firmRepresentativeEmail = recipient.FirmRepresentativeDetail?.EmailAddress;

            if (string.IsNullOrEmpty(firmRepresentativeEmail))
            {
                throw new NullReferenceException(
                    $"The recipient's firm representative email is required in the front end. Why it is null in DB?");
            }

            _emailService.SendEmail(firmRepresentativeEmail, subject, htmlContent);
            recipient.IsDirectDebitFollowupSent = true;
            recipient.EmbeddedDirectDebitSigning = embeddedSigningLink;
            recipient.IsInProgressDirectDebitFollowup = false;
            await _customerRepository.SaveCustomerAsync(recipient);
        }

        private static string GetViewDirectDebitMandateUrl(Customer recipient)
        {
            var queryStringBase64 = Helpers.EncodeToBase64($"email={recipient.Email}");
            return $"{Environment.GetEnvironmentVariable("BaseRedirectUrl", EnvironmentVariableTarget.Process)}" +
                   $"/direct-debit-sign-view?{queryStringBase64}";
        }

        private async Task<bool> CheckFulfilledStatusFromDocumentAsync(ILogger log)
        {
            log.LogInformation("Checking fulfilled status from document...");

            try
            {
                var username = Environment.GetEnvironmentVariable("Email", EnvironmentVariableTarget.Process);

                if (string.IsNullOrEmpty(username))
                {
                    throw new NullReferenceException(
                        $"The username should have value in {nameof(CheckFulfilledStatusFromDocumentAsync)}");
                }

                var isAuthenticated = await _signatureService.AuthenticateAsync(username, Password);

                if (!isAuthenticated)
                {
                    throw new UnauthorizedAccessException(
                        $"{nameof(CheckFulfilledStatusFromDocumentAsync)}: Not authorized to use Sign now.");
                }

                var modifiedDocuments =
                    (await _signatureService.GetModifiedDocumentsAsync(username, Password)).ToList();

                if (!modifiedDocuments.Any())
                {
                    log.LogInformation("No modified documents found");
                    return true;
                }

                foreach (var document in modifiedDocuments)
                {
                    var existingRecord = await _saveFieldsRepository.GetSaveFieldsByIdAsync(document.Id);

                    if (existingRecord)
                    {
                        continue;
                    }

                    await _saveFieldsRepository.SaveFieldsAsync(document);
                }

                log.LogInformation("Ended fulfilled status from document");
            }
            catch (SignNowAuthException ex)
            {
                log.LogError("Error occured at {SendProposalEmailAsyncName}: {ExMessage}",
                    nameof(SendProposalEmailAsync), ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                log.LogError("Error occured at {SendProposalEmailAsyncName}: {ExMessage}",
                    nameof(SendProposalEmailAsync), ex.Message);
            }
            catch (Exception ex)
            {
                log.LogError("Error occured at {CheckFulfilledStatusFromDocumentAsyncName}: {ExMessage}",
                    nameof(CheckFulfilledStatusFromDocumentAsync), ex.Message);
            }

            return true;
        }

        private static void SetChangeInfo(ChangeInfo customer)
        {
            customer.ChangedBy = ChangeSource.JobSignUp.ToString();
            customer.ChangedOn = DateHelper.GetCurrentDateTimeInEpoch();
            customer.IpAddress = GetIpAddress();
        }

        private static string GetIpAddress()
        {
            var hostName = Dns.GetHostName();
            var ipAddresses = Dns.GetHostAddresses(hostName);
            var ipAddress = ipAddresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return ipAddress != null ? ipAddress.ToString() : "";
        }
    }
}