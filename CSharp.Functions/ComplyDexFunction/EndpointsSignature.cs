using Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Common;

namespace Api
{
    public partial class Endpoints
    {
        //This is all about Signature Service
        //like we can use signnow or docsign

        // TODO. To delete soon. 
        // still used in Vue app (DocumentEdit.vue 'SignAsync' method)
        [FunctionName(nameof(SendInviteDocumentSignAsync))]
        public async Task<IActionResult> SendInviteDocumentSignAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var receiverEmail = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(receiverEmail))
            {
                throw new NullReferenceException($"{nameof(receiverEmail)} should not be null.");
            }

            var username = Environment.GetEnvironmentVariable("Email", EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(username))
            {
                throw new NullReferenceException(
                    $"username should have value in {nameof(SendInviteDocumentSignAsync)}");
            }

            var isAuthenticated = await _signatureService.AuthenticateAsync(username, SignNowPassword);

            if (!isAuthenticated)
            {
                return new UnauthorizedResult();
            }

            // generate html source and generate pdf
            var selectedHtmlSource = await _htmlRepository.GetHtmlSourcesAsync(DocumentNames.Proposal.ToString());

            if (selectedHtmlSource == null)
            {
                throw new ArgumentNullException(nameof(selectedHtmlSource),
                    $"The html source should not be null in {nameof(SendInviteDocumentSignAsync)}");
            }

            // Modify proposal document here
            var cleanedReceiverEmail = CleanStringFromExcessDoubleQuotes(receiverEmail);
            var customer = await _customerRepository.GetCustomerByEmailAsync(cleanedReceiverEmail);

            if (customer == null)
            {
                throw new NullReferenceException(
                    $"The customer with email {cleanedReceiverEmail} should already exists in database");
            }

            selectedHtmlSource.Content = await ReplaceDocumentVariables(selectedHtmlSource.Content, customer);
            var pdfUrl = await _pdfService.ConvertToPdfAsync(selectedHtmlSource);
            var signingLink = await _signatureService.SendDocSignInvite(customer, pdfUrl, false);
            return new OkObjectResult(signingLink?.Link);
        }

        [FunctionName(nameof(PostProcessSignedDocumentAsync))]
        public async Task<IActionResult> PostProcessSignedDocumentAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var username = Environment.GetEnvironmentVariable("Email", EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(username))
            {
                throw new NullReferenceException(
                    $"username should have value in {nameof(PostProcessSignedDocumentAsync)}");
            }

            var isAuthenticated = await _signatureService.AuthenticateAsync(username, SignNowPassword);

            if (!isAuthenticated)
            {
                return new UnauthorizedResult();
            }

            var param = req.Query["Param"];
            dynamic jsonObject = JObject.Parse(param);
            string docIdValue = jsonObject.docId;
            var document = await _signatureService.GetDocumentByIdAsync(docIdValue)
                           ?? throw new Exception(
                               $"Document with id {docIdValue} not found in {nameof(PostProcessSignedDocumentAsync)}!");
            var recipientEmail = document.FieldInvites.FirstOrDefault()?.Email;

            if (string.IsNullOrEmpty(recipientEmail))
            {
                throw new Exception(
                    $"Document with id {docIdValue} has no recipient in {nameof(PostProcessSignedDocumentAsync)}!");
            }

            if (recipientEmail.Contains(" ("))
            {
                recipientEmail = recipientEmail[..recipientEmail.IndexOf(" (", StringComparison.Ordinal)];
            }

            // get the html from blob storage
            var blobStorageBaseUrl =
                Environment.GetEnvironmentVariable("AzureStorageBaseUrl", EnvironmentVariableTarget.Process);
            var blobContainerName =
                Environment.GetEnvironmentVariable("BlobStorageContainerName", EnvironmentVariableTarget.Process);
            var htmlContents =
                $"{blobStorageBaseUrl}{blobContainerName}/EmailTemplates/ProposalConsultancyAgreement/account.opening.confirmation.html";
            using var client = new HttpClient();
            var bodyContent = await client.GetStringAsync(htmlContents);
            var customer = await _customerRepository.GetCustomerByEmailAsync(recipientEmail);

            if (customer == null)
            {
                throw new NullReferenceException(
                    $"The customer with email {recipientEmail} should already exists in database");
            }

            SetChangeInfo(customer, req);
            var consultancyName = (await _settingRepository.GetAllSettingAsync())
                                  .FirstOrDefault(s => s.Key == "$(CONSULTANCY_NAME)")?.Value ??
                                  throw new NullReferenceException(
                                      "Consultancy name should be in settings container");
            var settingsList = (await _settingRepository.GetAllSettingAsync()).ToList();
            var fcaRegulationUrl = settingsList.FirstOrDefault(s => s.Key == "$(FCARegulationUrl)");
            var changePasswordFlowUrl = GetChangePasswordFlowUrl(customer);
            var expirationDate = GetExpirationDate(document);
            var consultancyMobileNumber = settingsList.FirstOrDefault(s => s.Key == "$(CONSULTANCY_MOBILE_NO)")
                ?.Value;

            bodyContent = bodyContent.Replace("$(ConsultancyName)", consultancyName);
            bodyContent = bodyContent.Replace("$(RepresentativeName)", customer.FirstName + " " + customer.LastName);
            bodyContent = bodyContent.Replace("$(FCARegulationUrl)", fcaRegulationUrl?.Value ?? "");
            bodyContent = bodyContent.Replace("$(LoginUrl)", changePasswordFlowUrl);
            bodyContent = bodyContent.Replace("$(Date)", expirationDate.ToString(AppConstants.DateFormatDefault));
            bodyContent = bodyContent.Replace("$(Time)", expirationDate.ToString("hh:mm tt"));
            bodyContent = bodyContent.Replace("$(TempPassword)", $"{customer.TempPassword}");
            bodyContent = bodyContent.Replace("$(CONSULTANCY_MOBILE_NO)", consultancyMobileNumber);
            bodyContent = bodyContent.Replace("$(PlatformName)", consultancyName);

            //add scheduling here the first one
            var applicationUserAccount = await _calendlyService.GetApplicationAccountAsync();

            if (!string.IsNullOrEmpty(applicationUserAccount?.Resource?.UserUrl))
            {
                var eventTypes = await _calendlyService.GetEventTypesAsync(applicationUserAccount.Resource.UserUrl);
                var calendarUrl = eventTypes?.Collection.FirstOrDefault(x => x.Active)?.SchedulingUrl ?? "";
                bodyContent = bodyContent.Replace("$(CalendarUrl)", calendarUrl);
            }

            // download the signed document
            var link = await _signatureService.GetDocumentDownloadLinkAsync(docIdValue);
            var pdfFileResponse = await client.GetAsync(new Uri(link));
            var file = await pdfFileResponse.Content.ReadAsStreamAsync();
            await using (file)
            {
                const string subject = "Account Opening Confirmation";
                _emailService.SendEmail(recipientEmail, subject, bodyContent, file,
                    "ProposalAndConsultancyAgreement.pdf");
                customer.IsProposalDocumentSigned = true;
                await _customerRepository.SaveCustomerAsync(customer);

                if (string.IsNullOrEmpty(customer.Email))
                {
                    throw new NullReferenceException(
                        $"This customer {recipientEmail} should be no instance it dont have email prop stored in db.");
                }

                // update colleagues
                var colleagues = await _customerRepository.GetColleaguesAsync(customer.Email);

                foreach (var colleague in colleagues)
                {
                    colleague.IsProposalDocumentSigned = true;
                    SetChangeInfo(colleague, req);

                    if (colleague.EmbeddedSigning == null)
                    {
                        await _customerRepository.SaveCustomerAsync(colleague);
                        continue;
                    }

                    colleague.EmbeddedSigning.SignedByColleagueEmail = customer.Email;

                    if (customer.EmbeddedSigning?.Link != null)
                    {
                        colleague.EmbeddedSigning.Link = customer.EmbeddedSigning?.Link;
                    }

                    log.LogWarning(
                        "Updating {ColleagueEmail}\'s embedded sign link because colleague \'{CustomerEmail}\' signed the proposal for company \'{CustomerCompanyName}\'...",
                        colleague.Email, customer.Email, customer.CompanyName);
                    await _customerRepository.SaveCustomerAsync(colleague);
                }

                return new OkObjectResult(true);
            }
        }

        [FunctionName(nameof(PostProcessDeclinedDocumentAsync))]
        public async Task<IActionResult> PostProcessDeclinedDocumentAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var username = Environment.GetEnvironmentVariable("Email", EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(username))
            {
                throw new NullReferenceException(
                    $"username should have value in {nameof(PostProcessSignedDocumentAsync)}");
            }

            var isAuthenticated = await _signatureService.AuthenticateAsync(username, SignNowPassword);

            if (!isAuthenticated)
            {
                return new UnauthorizedResult();
            }

            var param = req.Query["Param"];
            dynamic jsonObject = JObject.Parse(param);
            string docIdValue = jsonObject.docId;
            var document = await _signatureService.GetDocumentByIdAsync(docIdValue)
                           ?? throw new Exception(
                               $"Document with id {docIdValue} not found in {nameof(PostProcessSignedDocumentAsync)}!");
            var recipientEmail = document.FieldInvites.FirstOrDefault()?.Email;

            if (string.IsNullOrEmpty(recipientEmail))
            {
                throw new Exception(
                    $"Document with id {docIdValue} has no recipient in {nameof(PostProcessSignedDocumentAsync)}!");
            }

            if (recipientEmail.Contains(" ("))
            {
                recipientEmail = recipientEmail[..recipientEmail.IndexOf(" (", StringComparison.Ordinal)];
            }

            log.LogWarning("Document with document id {DocIdValue} is rejected by {RecipientEmail}", docIdValue,
                recipientEmail);
            var customer = await _customerRepository.GetCustomerByEmailAsync(recipientEmail);

            if (customer == null)
            {
                throw new NullReferenceException(
                    $"The customer {recipientEmail} should exists already in the database.");
            }

            SetChangeInfo(customer, req);
            customer.IsProposalDocumentRejected = true;
            await _customerRepository.SaveCustomerAsync(customer);
            return new OkObjectResult(true);
        }

        private static DateTime GetExpirationDate(DocumentK document)
        {
            var expirationTime = document.FieldInvites.FirstOrDefault()?.ExpirationTime;
            var expirationDate = DateTime.UtcNow;

            if (long.TryParse(expirationTime, out var expirationTimeLong))
            {
                expirationDate = DateHelper.ConvertEpochToDateTime(expirationTimeLong);
            }

            return expirationDate;
        }

        private async Task<string> ReplaceDocumentVariables(string htmlContent, Customer customer)
        {
            if (customer == null)
            {
                throw new NullReferenceException($"Customer should have value in {nameof(ReplaceDocumentVariables)}");
            }

            var regex = new Regex(@"\$\(\w+\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var documentVariables = regex.Matches(htmlContent).Distinct();
            var addedPermissions = customer.CustomerPermissions
                .Where(cp => cp.IsModified && cp.State == "Added")
                .ToList();
            var hasAddedPermissions = addedPermissions.Any();
            var settings = await _settingRepository.GetAllSettingAsync();
            var settingsList = settings.ToList();

            if (!(settings != null && settingsList.Any()))
            {
                throw new NullReferenceException("The settings container should have values");
            }

            foreach (var variable in documentVariables)
            {
                var value = string.Empty;
                Setting foundSetting;

                switch (variable.ToString().ToUpper())
                {
                    case "$(DATE_GENERATED)":
                        value = DateTime.UtcNow.ToString("dd MM yyyy");
                        break;
                    case "$(CONSULTANCY_NAME)":
                        foundSetting = settingsList.FirstOrDefault(s => s.Key == "$(CONSULTANCY_NAME)");
                        value = foundSetting?.Value;
                        break;
                    case "$(FIRM_NAME)":
                        if (customer.IsCompanyNotApplicable)
                        {
                            var customerFirstName = customer.FirstName ??
                                                    throw new NullReferenceException(
                                                        $"first name should not be null in {nameof(ReplaceDocumentVariables)}");
                            var customerLastName = customer.LastName ??
                                                   throw new NullReferenceException(
                                                       $"last name should not be null in {nameof(ReplaceDocumentVariables)}");
                            value = $"{customerFirstName} {customerLastName}";
                        }
                        else
                        {
                            value = customer.SelectedCompany?.CompanyName ??
                                    throw new NullReferenceException(
                                        $"company name should not be null in {nameof(ReplaceDocumentVariables)}");
                        }

                        break;
                    case "$(PROPOSED)":
                        foundSetting = settingsList.FirstOrDefault(s => s.Key == "$(PROPOSED)");
                        value = hasAddedPermissions ? foundSetting?.Value : string.Empty;
                        break;
                    case "$(SUPPORT_WITH_DRAFTING)":
                        foundSetting = settingsList.FirstOrDefault(s => s.Key == "$(SUPPORT_WITH_DRAFTING)");
                        value = hasAddedPermissions ? foundSetting?.Value : string.Empty;
                        break;
                    case "$(FCA_CURRENT_PERMISSIONS)":
                        var authorisedPermissions = customer.CurrentFcaPermissions
                            .Where(p => p.State == "Added");

                        if (authorisedPermissions.Any())
                        {
                            var currentPermissionsList = customer.CurrentFcaPermissions.Where(p => p.State == "Added")
                                .Aggregate(string.Empty,
                                    (current, item) =>
                                        current +
                                        $"<li>{(string.IsNullOrWhiteSpace(item.SubPermissionName) ? item.PermissionGroupName : item.SubPermissionName)}</li>");

                            foundSetting = settingsList.FirstOrDefault(s => s.Key == "$(FCA_CURRENT_PERMISSIONS)");

                            if (foundSetting != null)
                            {
                                value = string.Format(foundSetting.Value, currentPermissionsList);
                            }
                        }
                        else
                        {
                            value = string.Empty;
                        }

                        break;
                    case "$(FCA_REQUESTED_CLAUSE1)":
                        foundSetting = settingsList.FirstOrDefault(s => s.Key == "$(FCA_REQUESTED_CLAUSE1)");
                        value = hasAddedPermissions ? foundSetting?.Value : string.Empty;
                        break;
                    case "$(REQUESTED_FCA_PERMISSIONS)":
                        if (hasAddedPermissions)
                        {
                            var permissionListStr = addedPermissions.Aggregate(string.Empty,
                                (current, item) =>
                                    current +
                                    $"<li>{(string.IsNullOrWhiteSpace(item.SubPermissionName) ? item.PermissionGroupName : item.SubPermissionName)}</li>");
                            foundSetting = settingsList.FirstOrDefault(s => s.Key == "$(REQUESTED_FCA_PERMISSIONS)");

                            if (foundSetting != null)
                            {
                                value = string.Format(foundSetting.Value, permissionListStr);
                            }
                        }
                        else
                        {
                            value = string.Empty;
                        }

                        break;
                    case "$(FCA_REQUESTED_CLAUSE2)":
                        var additionalText = string.Empty;

                        if (hasAddedPermissions)
                        {
                            additionalText = settingsList.FirstOrDefault(s => s.Key == "$(AUTHORIZATION_WITH_FCA)")
                                ?.Value;
                        }

                        foundSetting = settingsList.FirstOrDefault(s => s.Key == "$(FCA_REQUESTED_CLAUSE2)");

                        if (foundSetting != null)
                        {
                            value = string.Format(foundSetting.Value, additionalText);
                        }

                        break;
                    case "$(FCA_REQUESTED_CLAUSE3)":
                        foundSetting = settingsList.FirstOrDefault(s => s.Key == "$(FCA_REQUESTED_CLAUSE3)");

                        if (foundSetting != null)
                        {
                            value = hasAddedPermissions ? foundSetting.Value : string.Empty;
                        }

                        break;
                }

                htmlContent = htmlContent.Replace(variable.ToString(), value);
            }

            return htmlContent;
        }

        //These 2 endpoints used for exposing testing but not yet used for now in
        //frontend
        [FunctionName(nameof(SendEmbeddedInviteAsync))]
        public async Task<IActionResult> SendEmbeddedInviteAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var username = Environment.GetEnvironmentVariable("Email", EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(username))
            {
                throw new NullReferenceException(
                    $"username should have value in {nameof(SendEmbeddedInviteAsync)}");
            }

            var isAuthenticated = await _signatureService.AuthenticateAsync(username, SignNowPassword);

            if (!isAuthenticated)
            {
                return new UnauthorizedResult();
            }

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var embeddedInvites = JsonConvert.DeserializeObject<EmbeddedInviteK>(requestBody);
            var embeddedInvite = embeddedInvites.Invites.FirstOrDefault();
            var documentId = req.Query["docId"];

            var fieldInvites =
                await _signatureService.CreateEmbeddedInviteAsync(documentId, embeddedInvite.Email,
                    new List<string> { embeddedInvite.RoleId });
            return new OkObjectResult(fieldInvites);
        }

        [FunctionName(nameof(CreateEmbeddedSigningLinkAsync))]
        public async Task<IActionResult> CreateEmbeddedSigningLinkAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var username = Environment.GetEnvironmentVariable("Email", EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(username))
            {
                throw new NullReferenceException(
                    $"username should have value in {nameof(SendEmbeddedInviteAsync)}");
            }

            var isAuthenticated = await _signatureService.AuthenticateAsync(username, SignNowPassword);

            if (!isAuthenticated)
            {
                return new UnauthorizedResult();
            }

            var documentId = req.Query["docId"];
            var fieldInviteId = req.Query["fieldInviteId"];

            var embeddedSigning = await _signatureService.CreateEmbeddedInviteLinkAsync(documentId, fieldInviteId);
            return new OkObjectResult(embeddedSigning.Link);
        }

        private static string GetChangePasswordFlowUrl(Customer recipient)
        {
            var queryStringBase64 = Helpers.EncodeToBase64($"email={recipient.Email}");
            return $"{Environment.GetEnvironmentVariable("BaseRedirectUrl", EnvironmentVariableTarget.Process)}" +
                   $"/change-password?{queryStringBase64}";
        }
    }
}