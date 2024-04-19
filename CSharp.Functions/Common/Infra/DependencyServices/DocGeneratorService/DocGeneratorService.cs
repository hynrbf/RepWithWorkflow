using Common.Entities;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Aspose.Words;
using Azure.Storage.Blobs;
using System.Text.RegularExpressions;
using Aspose.Words.Replacing;

namespace Common.Infra
{
    public class DocGeneratorService : AsposeLicensedServiceBase, IDocGeneratorService
    {
        private const string Version = "1.0";
        private const string BasePath = "ConversionProcess";
        private const string PathToSavePdf = $"{BasePath}/pdf.files";
        private const string PathToSaveHtml = $"{BasePath}/html.files";
        private static string? CurrentHtmlPath { get; set; }

        private readonly IHtmlRepository _htmlRepository;
        private readonly ISettingRepository _settingRepository;

        private string _rootDirectory;
        private BlobContainerClient? _blobContainerClient;

        public DocGeneratorService(IHtmlRepository htmlRepository,
            ISettingRepository settingRepository) : base()
        {
            _htmlRepository = htmlRepository;
            _settingRepository = settingRepository;
        }

        public async Task GenerateDocumentAsync(Customer customer, string documentName, ILogger log,
            BlobContainerClient blobContainerClient, string rootDirectory)
        {
            log.LogInformation("{GenerateDocumentAsyncName}: Getting documents of {CustomerEmail} from cosmos db...",
                nameof(GenerateDocumentAsync), customer.Email);
            _rootDirectory = rootDirectory;
            _blobContainerClient = blobContainerClient;
            var consultancyName = (await _settingRepository.GetSettingByKeyAsync("$(CONSULTANCY_NAME)")).Value;
            var docModel = await _htmlRepository.GetHtmlSourceAsync(documentName, Version, consultancyName);

            if (string.IsNullOrEmpty(docModel.FileUrl))
            {
                throw new DocumentNotFoundInDBException(
                    $"This {consultancyName} has no document setup for html content document db and failed to generate to {customer.Email}");
            }

            var documentFileUrl = docModel.FileUrl;
            var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var tempPathToSaveHtml = $"{baseDirectory}/{PathToSaveHtml}/{customer.Email}";
            var tempPathToSavePdf = $"{baseDirectory}/{PathToSavePdf}/{customer.Email}";
            await ConvertWordToHtmlAsync(customer, documentFileUrl, log, tempPathToSaveHtml);
            await ConvertHtmlToPdfAsync(log, tempPathToSavePdf);

            // Delete generated files here
            DeleteDirectory(tempPathToSaveHtml, log);
            DeleteDirectory(tempPathToSavePdf, log);
        }

        private async Task UploadFilesToAzureBlobStorageAsync(string name, string inputPath, ILogger log)
        {
            if (_blobContainerClient == null)
            {
                log.LogWarning(
                    $"{nameof(UploadFilesToAzureBlobStorageAsync)}: Uploading files to Blob Storage aborted!");
                return;
            }

            var pathSegment = inputPath.Split('\\');
            var folderName = $"ConversionProcess\\{pathSegment[^2]}\\{pathSegment[^1]}";

            log.LogInformation($"{nameof(UploadFilesToAzureBlobStorageAsync)}: Uploading files ...");
            // Iterate through files in the folder
            foreach (var filePath in Directory.GetFiles(inputPath))
            {
                // Do not upload not related files
                if (!filePath.Contains(name))
                {
                    continue;
                }

                var fileName = Path.GetFileName(filePath);
                var fileNamePath = $"{folderName}\\{fileName}";
                var blobClient = _blobContainerClient.GetBlobClient(fileNamePath);
                await using var fileStream = File.OpenRead(filePath);

                log.LogInformation("\tUploading {FileName} ...", fileName);
                await blobClient.UploadAsync(fileStream, true);
            }

            log.LogInformation("\tUpload complete!");
        }

        private static void DeleteDirectory(string directoryPath, ILogger log)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            log.LogWarning("Deleting files in \'{DirectoryPath}\' ...", directoryPath);
            Directory.Delete(directoryPath, true);
        }

        private void SetFont()
        {
            var fontsPath = Path.Combine(_rootDirectory, "fonts");
            Aspose.Words.Fonts.FontSettings.DefaultInstance.SetFontsFolder(fontsPath, false);
        }

        #region Word to Html, then Html to Pdf

        private async Task ConvertHtmlToPdfAsync(ILogger log, string tempPathToSavePdf)
        {
            if (string.IsNullOrEmpty(CurrentHtmlPath))
            {
                throw new NullReferenceException(
                    $"Current html path should not be null here at {nameof(ConvertHtmlToPdfAsync)}");
            }

            SetFont();
            var fileName = Path.GetFileNameWithoutExtension(CurrentHtmlPath);
            var doc = new Document(CurrentHtmlPath);

            var fullFileName = $"{tempPathToSavePdf}/{fileName}.pdf";
            doc.Save(fullFileName);

            var directoryToUpload = Path.GetDirectoryName(fullFileName);

            if (string.IsNullOrEmpty(directoryToUpload))
            {
                throw new Exception("Directory should not be null here!");
            }

            await UploadFilesToAzureBlobStorageAsync(fileName, directoryToUpload, log);
        }

        private async Task ConvertWordToHtmlAsync(Customer customer, string wordDocFileUrl, ILogger log,
            string tempPathToSaveHtml)
        {
            var fileName = Path.GetFileNameWithoutExtension(wordDocFileUrl);
            var doc = new Document(wordDocFileUrl);
            doc = await ReplaceVariablesAsync(fileName, doc, customer);

            var fullFileName = $"{tempPathToSaveHtml}/{fileName}.html";
            CurrentHtmlPath = fullFileName;
            doc.Save(fullFileName);

            var directoryToUpload = Path.GetDirectoryName(fullFileName);

            if (string.IsNullOrEmpty(directoryToUpload))
            {
                throw new Exception("Directory should not be null here!");
            }

            await UploadFilesToAzureBlobStorageAsync(fileName, directoryToUpload, log);
        }

        #endregion

        #region Replacing Variables

        private async Task<Document> ReplaceVariablesAsync(string fileName, Document document, Customer customer)
        {
            if (fileName == DocumentNames.Proposal.ToString())
            {
                return await ReplaceWordDocumentVariablesForProposalAsync(document, customer);
            }

            if (fileName == DocumentNames.DirectDebitMandate.ToString())
            {
                return await ReplaceWordDocumentVariablesForDirectDebitMandateAsync(document, customer);
            }

            return document;
        }

        private async Task<Document> ReplaceWordDocumentVariablesForProposalAsync(Document document,
            Customer customer)
        {
            if (customer == null)
            {
                throw new NullReferenceException(
                    $"Customer should have value in {nameof(ReplaceWordDocumentVariablesForProposalAsync)}");
            }

            var regex = new Regex(@"\$\(\w+\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var htmlContent = document.Range.Text;
            var documentVariables = regex.Matches(htmlContent).Distinct();
            var addedPermissions = customer.CustomerPermissions
                .Where(cp => cp.IsModified && cp.State == "Added")
                .ToList();
            var hasAddedPermissions = addedPermissions.Any();
            var settings = await _settingRepository.GetAllSettingAsync();
            var settingsList = settings.ToList();

            if (!(settingsList != null && settingsList.Any()))
            {
                throw new NullReferenceException("The settings container should have values");
            }

            foreach (var variable in documentVariables)
            {
                var value = string.Empty;
                Setting? foundSetting;

                switch (variable.ToString().ToUpper())
                {
                    case "$(DATE_GENERATED)":
                        value = DateTime.UtcNow.ToString("dd/MM/yyyy");
                        break;
                    case "$(CONSULTANCY_NAME)":
                        foundSetting = settingsList.FirstOrDefault(s => s.Key == "$(CONSULTANCY_NAME)") ??
                                       throw new NullReferenceException(
                                           "Consultancy name should be in settings container");
                        value = foundSetting.Value;
                        break;
                    case "$(CLIENT_PARTY)":
                        if (customer.IsCompanyNotApplicable)
                        {
                            // Sole trader
                            var fullName = $"{customer.FirstName} {customer.LastName}";
                            var convert =
                                DateHelper.ConvertEpochToDateTime(customer.SoleTraderDetails?.DateOfBirthInEpoch ?? 0);
                            var dateOfBirth = $"Born on {convert:dd/MM/yyyy} ";
                            value = $"{fullName}, Residing at {customer.SoleTraderDetails?.HomeAddress}, {dateOfBirth}";
                        }
                        else
                        {
                            // company
                            var companyName =
                                GetCompanyName(customer.CompanyName ?? ""); //company name can be null in db
                            var companyAddress = customer.CompanyAddress ?? ""; //company address can be null in db
                            value =
                                $"{companyName} incorporated and registered in England and Wales with company number {customer.CompanyNumber} whose registered office is at {companyAddress}";
                        }

                        break;
                    case "$(FIRM_NAME)":
                        if (customer.IsCompanyNotApplicable)
                        {
                            var customerFirstName = customer.FirstName ??
                                                    throw new NullReferenceException(
                                                        $"first name should not be null in {nameof(ReplaceWordDocumentVariablesForProposalAsync)}");
                            var customerLastName = customer.LastName ??
                                                   throw new NullReferenceException(
                                                       $"last name should not be null in {nameof(ReplaceWordDocumentVariablesForProposalAsync)}");
                            value = $"{customerFirstName} {customerLastName}";
                        }
                        else
                        {
                            value = customer.SelectedCompany?.CompanyName ?? "";
                            value = GetCompanyName(value); //company name can be null in db
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
                    case "$(COMMENCEMENT_DATE)":
                        // <XXX Day of the Month of XXX 20xx (xx month 20xx)>
                        var dateUtcNow = DateTime.UtcNow;
                        var ordinalDay = Helpers.GetOrdinal(dateUtcNow.Day);
                        var day = dateUtcNow.ToString("dd");
                        var month = dateUtcNow.ToString("MMM");
                        var year = dateUtcNow.Year;
                        value = $"{ordinalDay} Day of the Month of {month} {year} ({day} {month} {year})";
                        break;
                    case "$(COMMENCEMENT_DATE_2)":
                        // <XXX day of the Month of XXX of the year 20xx>
                        var dateUtcNow2 = DateTime.UtcNow;
                        var ordinalDay2 = Helpers.GetOrdinal(DateTime.UtcNow.Day);
                        var day2 = dateUtcNow2.ToString("dd");
                        var month2 = dateUtcNow2.ToString("MMM");
                        var year2 = dateUtcNow2.Year;
                        value = $"{ordinalDay2} day of the Month of {month2} of the year {year2}";
                        break;
                }

                document = UpdateDocument(document, variable, value ?? "");
            }

            return document;
        }

        private static string GetCompanyName(string companyName)
        {
            if (string.IsNullOrEmpty(companyName))
            {
                return "";
            }

            if (companyName.Contains(" ("))
            {
                companyName = companyName[..companyName.IndexOf(" (", StringComparison.Ordinal)];
            }

            return companyName;
        }

        private async Task<Document> ReplaceWordDocumentVariablesForDirectDebitMandateAsync(Document document,
            Customer customer)
        {
            if (customer == null)
            {
                throw new NullReferenceException(
                    $"Customer should have value in {nameof(ReplaceWordDocumentVariablesForDirectDebitMandateAsync)}");
            }

            var regex = new Regex(@"\$\(\w+\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var htmlContent = document.Range.Text;
            var documentVariables = regex.Matches(htmlContent).Distinct();

            var settings = await _settingRepository.GetAllSettingAsync();
            var settingsList = settings.ToList();
            string companyName;

            if (customer.IsCompanyNotApplicable)
            {
                var customerFirstName = customer.FirstName ??
                                        throw new NullReferenceException(
                                            $"first name should not be null in {nameof(ReplaceWordDocumentVariablesForDirectDebitMandateAsync)}");
                var customerLastName = customer.LastName ??
                                       throw new NullReferenceException(
                                           $"last name should not be null in {nameof(ReplaceWordDocumentVariablesForDirectDebitMandateAsync)}");

                companyName = $"{customerFirstName} {customerLastName}";
            }
            else
            {
                companyName = customer.SelectedCompany?.CompanyName ??
                              throw new NullReferenceException(
                                  $"company name should not be null in {nameof(ReplaceWordDocumentVariablesForDirectDebitMandateAsync)}");
            }

            foreach (var variable in documentVariables)
            {
                var value = string.Empty;
                Setting? foundSetting;

                switch (variable.ToString().ToUpper())
                {
                    case "$(FULLNAME)":
                        var fullName = $"{customer.FirstName} {customer.LastName}";
                        value = fullName ?? throw new NullReferenceException(
                            $"The Direct Debit Name should have value in {nameof(ReplaceWordDocumentVariablesForDirectDebitMandateAsync)}");
                        break;
                    case "$(COMPANYNAME)":
                        value = companyName.RemovePostCodeString();
                        break;
                    case "$(COMPANY_ADDRESS)":
                        value = customer.CompanyAddress;
                        break;
                    case "$(POSTCODE)":
                        const string pattern = @"Postcode:\s*([a-zA-Z0-9\s]+)";
                        var match = Regex.Match(companyName, pattern);
                        value = match.Success ? match.Groups[1].Value.Trim() : string.Empty;
                        break;
                    case "$(DDMONTHYEAR)":
                        value = DateTime.UtcNow.ToString("dd MMMM yyyy");
                        break;
                    case "$(PLEASE_NOTE)":
                        foundSetting = settingsList.FirstOrDefault(s => s.Key == "$(PLEASE_NOTE)");
                        value = foundSetting?.Value;
                        break;
                    case "$(CONSULTANCY_NAME)":
                        foundSetting = settingsList.FirstOrDefault(s => s.Key == "$(CONSULTANCY_NAME)");
                        value = foundSetting?.Value;
                        break;
                    case "$(DIRECT_DEBIT_COL_REF)":
                        foundSetting = settingsList.FirstOrDefault(s => s.Key == "$(DIRECT_DEBIT_COL_REF)");
                        value = foundSetting?.Value;
                        break;
                }

                document = UpdateDocument(document, variable, value ?? "");
            }

            return document;
        }

        private static Document UpdateDocument(Document document, Match variable, string value)
        {
            var htmlRegex = new Regex(@"<[^>]+>");
            var hasHtmlTags = !string.IsNullOrEmpty(value) && htmlRegex.IsMatch(value);

            if (hasHtmlTags)
            {
                var options = new FindReplaceOptions
                {
                    ReplacingCallback = new ReplaceWithHtmlEvaluator()
                };
                document.Range.Replace(variable.ToString(), value, options);
            }
            else
            {
                document.Range.Replace(variable.ToString(), value);
            }

            return document;
        }

        #endregion
    }
}