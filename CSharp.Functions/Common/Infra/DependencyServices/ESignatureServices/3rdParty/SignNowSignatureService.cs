using Common.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace Common.Infra
{
    public class SignNowSignatureService : IThirdPartySigningService
    {
        private const string SignNowBaseUrl = "https://api-eval.signnow.com";
        private readonly HttpClient _httpClient = new();

        private string? _callBackBaseUrl;

        public SignNowSignatureService()
        {
            _httpClient.BaseAddress = new Uri(SignNowBaseUrl);
        }

        public async Task<string> GenerateTokenAsync(string userName, string password)
        {
            if (_httpClient.DefaultRequestHeaders.Authorization?.Parameter != null)
            {
                var oldToken = _httpClient.DefaultRequestHeaders.Authorization.Parameter;
                var isTokenExpired = await VerifyTokenAsync(oldToken);

                if (!isTokenExpired)
                {
                    return oldToken;
                }
            }

            var token = await GenerateTokenInternalAsync(userName, password);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return token;
        }

        public async Task<EmbeddedSigning?> SendInviteToSignAsync(Customer recipient, string pdfUrl,
            int signTextYIndent)
        {
            if (string.IsNullOrEmpty(recipient.Email))
            {
                throw new NullReferenceException(
                    "Customer or customer email should not be null in SignNowSignatureService.SendInviteToSignAsync");
            }

            var client = new HttpClient();
            var pdfFileResponse = await client.GetAsync(new Uri(pdfUrl));
            var fileStream = await pdfFileResponse.Content.ReadAsStreamAsync();
            string documentId;

            await using (fileStream)
            {
                var filename = Path.GetFileName(pdfUrl);
                documentId = await UploadDocumentAsync(fileStream, filename);
                var documentPageCount = await GetDocumentPageCountAsync(documentId);
                var signFields = await GetSignFields(Path.GetFileNameWithoutExtension(pdfUrl), documentPageCount,
                    signTextYIndent);

                if (Path.GetFileNameWithoutExtension(pdfUrl) != DocumentNames.DirectDebitMandate.ToString())
                {
                    var nameTextField = signFields.FirstOrDefault(f => f.Name == "Name");

                    if (nameTextField != null)
                    {
                        nameTextField.Value = $"{recipient.FirstName} {recipient.LastName}";
                    }
                }

                //Please note, if we want a non editable field this will work via Texts property
                //if we want editable fields this will work via Fields property
                //but if we want the two of them works together, it is not allowed right now
                //accdg to signnow support
                documentId = await AddSignatureFieldToDocumentAsync(documentId, new EditDocumentPayload
                {
                    Fields = signFields
                });
            }

            if (string.IsNullOrEmpty(documentId))
            {
                throw new NullReferenceException(
                    $"The document id should not be null in {nameof(SignNowSignatureService)}.{nameof(SendInviteToSignAsync)}");
            }

            var document = await GetDocumentByIdAsync(documentId)
                           ?? throw new Exception(
                               $"Document with id {documentId} not found in {nameof(SendInviteToSignAsync)}!");

            var roleIds = document.Roles.Select(r => r.Id) ??
                          throw new Exception(
                              $"Document with id {documentId} has no role id in {nameof(SendInviteToSignAsync)}!");

            var fieldInvites = await CreateEmbeddedInviteAsync(documentId, recipient.Email, roleIds.ToList());
            var fieldInviteId = fieldInvites.FirstOrDefault()?.Id ??
                                throw new Exception(
                                    $"Error in sending embedded invite. Field invite (id) should not be null in {nameof(SendInviteToSignAsync)}!");

            // TODO. Temp for now. Subscribe only for signed / declined documents EXCEPT 'direct debit mandate' since we don't have
            // saving of direct debit mandate yet.
            if (Path.GetFileNameWithoutExtension(pdfUrl) != DocumentNames.DirectDebitMandate.ToString())
            {
                await SubscribeToDocumentEventAsync(documentId);
            }

            var embeddedSigning = await CreateEmbeddedInviteLinkAsync(documentId, fieldInviteId);
            embeddedSigning.Created = embeddedSigning.Updated;
            return embeddedSigning;
        }

        public async Task<IEnumerable<FieldInviteK>> CreateEmbeddedInviteAsync(string documentId, string email,
            List<string> roleIds)
        {
            var payload = new EmbeddedInviteK
            {
                Invites = new List<InviteK>()
            };

            var invites = new List<InviteK>();
            var order = 1;

            foreach (var roleId in roleIds)
            {
                invites.Add(new InviteK
                {
                    Email = email,
                    RoleId = roleId,
                    AuthMethod = "email",
                    Order = order
                });

                order++;
            }

            payload.Invites = invites;
            var jsonContent = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonContent, null, "application/json");
            var response = await _httpClient.PostAsync($"{SignNowBaseUrl}/v2/documents/{documentId}/embedded-invites",
                content);
            var result = await response.Content.ReadAsStringAsync();
            var embeddedInviteResponse = JsonConvert.DeserializeObject<EmbeddedInviteResponseK>(result);
            return embeddedInviteResponse?.Data ?? new List<FieldInviteK>();
        }

        public async Task<EmbeddedSigning> CreateEmbeddedInviteLinkAsync(string documentId, string fieldInviteId)
        {
            const int linkValidityInMins = 45;
            var jsonContent = $"{{\"auth_method\": \"email\",\"link_expiration\": {linkValidityInMins}}}";
            var content = new StringContent(jsonContent, null, "application/json");
            var response =
                await _httpClient.PostAsync(
                    $"{SignNowBaseUrl}/v2/documents/{documentId}/embedded-invites/{fieldInviteId}/link", content);
            var result = await response.Content.ReadAsStringAsync();
            var resultJsonObj = JObject.Parse(result);

            var dateNow = DateTime.UtcNow;
            var expiry = dateNow.AddMinutes(linkValidityInMins);
            var embeddedSigning = new EmbeddedSigning
            {
                Updated = DateHelper.ConvertDateTimeToEpoch(dateNow),
                DocumentId = documentId,
                FieldInviteId = fieldInviteId,
                Expiry = DateHelper.ConvertDateTimeToEpoch(expiry),
                Link = resultJsonObj?["data"]?["link"]?.ToString()
            };

            return embeddedSigning;
        }

        public async Task<DocumentK?> GetDocumentByIdAsync(string documentId)
        {
            var response = await _httpClient.GetAsync($"{SignNowBaseUrl}/document/{documentId}");
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DocumentK>(result);
        }

        public async Task<string> GetDocumentDownloadLinkAsync(string documentId)
        {
            var content = new StringContent(string.Empty);
            var response =
                await _httpClient.PostAsync($"{SignNowBaseUrl}/document/{documentId}/download/link", content);
            var result = await response.Content.ReadAsStringAsync();
            dynamic jsonObject = JObject.Parse(result);
            string link = jsonObject.link;
            return link;
        }

        public void Register(string callBackBaseUrl)
        {
            _callBackBaseUrl = callBackBaseUrl;
        }

        private async Task<List<DocumentField>> GetSignFields(string documentName, int documentPageCount, int yIndent)
        {
            var blobStorageBaseUrl =
                Environment.GetEnvironmentVariable("AzureStorageBaseUrl", EnvironmentVariableTarget.Process);
            var blobContainerName =
                Environment.GetEnvironmentVariable("BlobStorageContainerName", EnvironmentVariableTarget.Process);
            var jsonFile = string.Empty;
            var fillableFieldsPageLocation = 0;

            if (documentName == DocumentNames.Proposal.ToString())
            {
                fillableFieldsPageLocation = documentPageCount - 1;
                jsonFile = "ProposalSignFields.json";
            }

            if (documentName == DocumentNames.DirectDebitMandate.ToString())
            {
                fillableFieldsPageLocation = 0;
                //please note in this json we put Validator_id: 059b068ef8ee5cc27e09ba79af58f9e805b7c2b3
                //which means dd/MM/yyyy accdg to their docs https://docs.signnow.com/docs/signnow/features#field
                jsonFile = "DirectDebitSignFields.json";
            }

            var jsonFileUrl =
                $"{blobStorageBaseUrl}{blobContainerName}/SignFields/{jsonFile}";

            using var client = new HttpClient();
            var jsonContents = await client.GetStringAsync(jsonFileUrl);

            if (string.IsNullOrEmpty(jsonContents))
            {
                throw new NullReferenceException(
                    $"The json content should have a value in {nameof(GetSignFields)}");
            }

            var signFields = JsonConvert.DeserializeObject<List<DocumentField>>(jsonContents)?
                .Select(documentField =>
                {
                    documentField.PageNumber = fillableFieldsPageLocation;
                    documentField.Y -= yIndent;
                    return documentField;
                }).ToList();

            return signFields ?? new List<DocumentField>();
        }

        private async Task<string> AddSignatureFieldToDocumentAsync(string documentId, EditDocumentPayload payload)
        {
            var jsonContent = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonContent, null, "application/json");
            var response = await _httpClient.PutAsync($"{SignNowBaseUrl}/document/{documentId}", content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException(
                    $"Error found at {nameof(SignNowSignatureService)} .{nameof(AddSignatureFieldToDocumentAsync)}: {result}");
            }

            var resultJsonObj = JObject.Parse(result);
            return resultJsonObj["id"]?.ToString() ?? string.Empty;
        }

        private async Task<string> UploadDocumentAsync(Stream stream, string fileName)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{SignNowBaseUrl}/document");
            var content = new MultipartFormDataContent
            {
                { new StreamContent(stream), "file", fileName }
            };
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException(
                    $"Error found at {nameof(SignNowSignatureService)} .{nameof(UploadDocumentAsync)}: {result}");
            }

            var resultJsonObj = JObject.Parse(result);
            return resultJsonObj["id"]?.ToString() ?? string.Empty;
        }

        private async Task<int> GetDocumentPageCountAsync(string documentId)
        {
            var response = await _httpClient.GetAsync($"{SignNowBaseUrl}/document/{documentId}");
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException(
                    $"Error found at {nameof(SignNowSignatureService)} .{nameof(GetDocumentPageCountAsync)}: {result}");
            }

            var resultJsonObj = JObject.Parse(result);
            return int.Parse(resultJsonObj["page_count"]?.ToString() ?? "0");
        }

        private async Task<string> GenerateTokenInternalAsync(string userName, string password)
        {
            var basicAuthorizationToken
                = Environment.GetEnvironmentVariable("SignNowBasicAuthorizationToken",
                    EnvironmentVariableTarget.Process);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{SignNowBaseUrl}/oauth2/token");
            request.Headers.Add("Authorization", $"Basic {basicAuthorizationToken}");
            var content = new MultipartFormDataContent
            {
                { new StringContent(userName), "username" },
                { new StringContent(password), "password" },
                { new StringContent("password"), "grant_type" },
                { new StringContent("*"), "scope" }
            };
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode || string.IsNullOrEmpty(result))
            {
                throw new SignNowAuthException("SignNow authentication failed.");
            }

            var jObject = JObject.Parse(result);
            var newToken = jObject["access_token"]?.ToString();
            return newToken ?? "";
        }

        private static async Task<bool> VerifyTokenAsync(string token)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{SignNowBaseUrl}/oauth2/token");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var response = await client.SendAsync(request);
            return response.StatusCode != System.Net.HttpStatusCode.OK;
        }

        private async Task SubscribeToDocumentEventAsync(string documentId)
        {
            if (string.IsNullOrEmpty(_callBackBaseUrl))
            {
                throw new NullReferenceException(
                    $"Callback Base url must not be null in {nameof(SignNowSignatureService)}.{nameof(SubscribeToDocumentEventAsync)}");
            }

            var listOfEventSubscriptions = new List<DocumentEventSubscriptionK>
            {
                new()
                {
                    EventName = "document.complete",
                    DocumentId = documentId,
                    Attributes = new EventAttributesK
                    {
                        //_callbackurl like https://az-func-api-sun.azurewebsites.net
                        CallBack = $"{_callBackBaseUrl}/api/PostProcessSignedDocumentAsync",
                        Headers = new AttributeHeaderK()
                    }
                },
                new()
                {
                    EventName = "document.fieldinvite.decline",
                    DocumentId = documentId,
                    Attributes = new EventAttributesK
                    {
                        CallBack = $"{_callBackBaseUrl}/api/PostProcessDeclinedDocumentAsync",
                        Headers = new AttributeHeaderK()
                    }
                }
            };

            foreach (var content in listOfEventSubscriptions
                         .Select(JsonConvert.SerializeObject)
                         .Select(jsonContent => new StringContent(jsonContent, null, "application/json")))
            {
                var response = await _httpClient.PostAsync($"{SignNowBaseUrl}/api/v2/events", content);

                if (response.StatusCode != System.Net.HttpStatusCode.Created)
                {
                    //ToDo. what do we need to do. Do we throw error?
                }
            }
        }
    }
}