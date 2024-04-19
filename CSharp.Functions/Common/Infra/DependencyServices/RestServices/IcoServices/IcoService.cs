using System.Globalization;
using Azure.Storage.Blobs.Models;
using Common.Entities;
using Newtonsoft.Json;
using System.Runtime.Caching;
using System.Text;

namespace Common.Infra
{
    public class IcoService : RestServiceBase, IIcoService
    {
        private readonly IWebContentsScrapping _webContentsScrapping;
        private readonly string _baseUrl = AppSettingsProvider.Instance.GetValue(AppConstants.RestBaseIcoApi);
        private readonly MemoryCache _memoryCache;
        private readonly IBlobContainerService _blobContainerClientService;

        private string _azureStorageConnectionString;
        private string _blobStorageContainerName;

        public IcoService(IWebContentsScrapping webContentsScrapping, IBlobContainerService blobContainerClientService)
        {
            _webContentsScrapping = webContentsScrapping;
            _blobContainerClientService = blobContainerClientService;
            _memoryCache = MemoryCache.Default;
        }

        protected override HttpRequestMessage CreateRequestMessageGet(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            return request;
        }

        protected override HttpRequestMessage CreateRequestMessagePost(string endpoint, HttpContent httpContent)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = httpContent
            };
            return request;
        }

        public async Task<IcoDetails> GetDetailsByReferenceAsync(string reference)
        {
            var endpoint = $"{_baseUrl}/ESDWebPages/Entry/{reference}";
            if (_memoryCache.Get(endpoint) is IcoDetails cachedValue)
            {
                return cachedValue;
            }

            var apiResponse = await GetRemoteAsync<string>(endpoint);
            var icoDetails = ExtractData(apiResponse);

            _memoryCache.AddOrGetExisting(endpoint, icoDetails,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));

            return icoDetails;
        }

        public async Task<IcoDetails> SearchDetailsAsync(IcoSearchInput searchInput)
        {
            var endpoint = $"{_baseUrl}/ESDWebPages/DoSearch";

            var cacheKey = endpoint + System.Text.Json.JsonSerializer.Serialize(searchInput);
            if (_memoryCache.Get(cacheKey) is IcoDetails cachedValue)
            {
                return cachedValue;
            }

            dynamic dynamicData = new
            {
                fieldname = searchInput.CompanyName ?? "",
                fieldregistration = searchInput.Reference ?? "",
                fieldaddress = searchInput.Address ?? "",
                fieldpostcode = searchInput.Postcode ?? ""
            };
            string jsonData = JsonConvert.SerializeObject(dynamicData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await PostRemoteAsync<string>(endpoint, content);

            // Get the exact company name if multiple results;
            const string multipleResultsIndicator = "Total Records found";
            if (response.Contains(multipleResultsIndicator))
            {
                var aTags = _webContentsScrapping.GetNodesFromHtmlByTag(response, "a", multipleResultsIndicator);
                var matchedNode = aTags?.SingleOrDefault(a =>
                    a.InnerText.Equals(searchInput.CompanyName, StringComparison.InvariantCultureIgnoreCase));
                if (matchedNode != null)
                {
                    var subEndpoint = _baseUrl + matchedNode.Attributes["href"]?.Value;
                    response = await GetRemoteAsync<string>(subEndpoint);
                }
            }

            var icoDetails = ExtractData(response);

            _memoryCache.AddOrGetExisting(cacheKey, icoDetails,
                DateTimeOffset.Now.AddMinutes(AppConstants.MemoryCacheOneDayDurationInMinutes));

            return icoDetails;
        }

        private IcoDetails ExtractData(string htmlContent)
        {
            var icoDetails = new IcoDetails
            {
                RegistrationReference = _webContentsScrapping
                    .GetContentFromHtmlByKeyword(htmlContent, "Registration reference:")
                    .FirstOrDefault() ?? "",
                PaymentTier = _webContentsScrapping.GetContentFromHtmlByKeyword(htmlContent, "Payment tier:")
                    .FirstOrDefault() ?? "",
                DataController = _webContentsScrapping.GetContentFromHtmlByKeyword(htmlContent, "Data controller:")
                    .FirstOrDefault() ?? "",
                OtherNames = _webContentsScrapping.GetContentFromHtmlByKeyword(htmlContent, "Other names:")
                    .FirstOrDefault() ?? "",
            };

            var dateRegisteredString = _webContentsScrapping
                .GetContentFromHtmlByKeyword(htmlContent, "Date registered:")
                .FirstOrDefault() ?? "";
            if (!string.IsNullOrEmpty(dateRegisteredString))
            {
                icoDetails.DateRegistered = ConvertDateStringToEpoch(dateRegisteredString);
            }

            var registrationExpiresString = _webContentsScrapping
                .GetContentFromHtmlByKeyword(htmlContent, "Registration expires:")
                .FirstOrDefault() ?? "";
            if (!string.IsNullOrEmpty(registrationExpiresString))
            {
                icoDetails.RegistrationExpires = ConvertDateStringToEpoch(registrationExpiresString);
            }

            var addressString = _webContentsScrapping.GetContentFromHtmlByKeyword(htmlContent, "Address:")
                .FirstOrDefault() ?? "";

            var addressLines = addressString.Split(new[] { '\n', '\r' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var address = new IcoAddress
            {
                AddressLine1 = addressLines.ElementAtOrDefault(0) ?? "",
                AddressLine2 = addressLines.ElementAtOrDefault(1) ?? "",
                AddressLine3 = addressLines.ElementAtOrDefault(2) ?? "",
                AddressLine4 = addressLines.ElementAtOrDefault(3) ?? "",
                AddressLine5 = addressLines.ElementAtOrDefault(4) ?? ""
            };

            if (addressLines.Length > 1)
            {
                address.PostCode = addressLines[^1].Trim();
            }

            icoDetails.Address = address;

            var dataProtectionOfficerString = _webContentsScrapping
                .GetContentFromHtmlByKeyword(htmlContent, "Data Protection Officer:")
                .FirstOrDefault() ?? "";

            var dataProtectionOfficerLines = dataProtectionOfficerString.Split(new[] { '\n', '\r' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var dataProtectionOfficer = new IcoDataProtectionOfficer
            {
                Name = dataProtectionOfficerLines.FirstOrDefault()?.Trim(),
                ContactNumber = new ContactNumber
                {
                    Number = dataProtectionOfficerLines.LastOrDefault()?.Trim()
                },
                EmailAddress = dataProtectionOfficerLines.Reverse().Skip(1).FirstOrDefault()?.Trim()
            };
            icoDetails.DataProtectionOfficer = dataProtectionOfficer;
            return icoDetails;
        }

        private static long? ConvertDateStringToEpoch(string dateString)
        {
            var cultureInfo = new CultureInfo("en-UK");
            if (DateTime.TryParse(dateString, cultureInfo, DateTimeStyles.NoCurrentDateDefault, out var dateValue))
            {
                return DateHelper.ConvertDateTimeToEpoch(dateValue);
            }

            return null;
        }

        public async Task<string> SaveRegistrationCertificate(string reference)
        {
            var fileUrl = $"{_baseUrl}/ESDWebPages/RegistrationCertificate/{reference}";

            var httpClient = new HttpClient();
            var pdfFileResponse = await httpClient.GetAsync(fileUrl);
            var file = await pdfFileResponse.Content.ReadAsStreamAsync();

            await using (file)
            {
                if (file.Length <= 0)
                {
                    return string.Empty;
                }

                _blobContainerClientService.Register(_azureStorageConnectionString, _blobStorageContainerName);

                var container = _blobContainerClientService.BlobContainerClient;
                var createResponse = await container.CreateIfNotExistsAsync();

                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                {
                    await container.SetAccessPolicyAsync(PublicAccessType.Blob);
                }

                var blob = container.GetBlobClient(
                    $"DocStore/ICO/{reference}/Registration-Certificate_{reference}.pdf");
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await blob.UploadAsync(file, new BlobHttpHeaders { ContentType = "application/pdf" });

                return blob.Uri.ToString();
            }
        }

        public void Register(string azureStorageConnectionString, string blobStorageContainerName)
        {
            _azureStorageConnectionString = azureStorageConnectionString;
            _blobStorageContainerName = blobStorageContainerName;
        }
    }
}