using Common.Entities;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Common.Infra
{
    public class SignatureService : ISignatureService
    {
        private readonly IPdfWriterService _pdfWriterService;
        private readonly IPdfParserService _pdfParserService;

        private IThirdPartySigningService _thirdPartySignatureService;

        public SignatureService(IPdfWriterService pdfWriterService, IPdfParserService pdfParserService)
        {
            _pdfWriterService = pdfWriterService;
            _pdfParserService = pdfParserService;
        }

        public void Register(IThirdPartySigningService thirdPartyService, string callBackBaseUrl)
        {
            _thirdPartySignatureService = thirdPartyService;
            _thirdPartySignatureService.Register(callBackBaseUrl);
        }

        public async Task<bool> AuthenticateAsync(string userName, string password)
        {
            if (_thirdPartySignatureService == null)
            {
                throw new NullReferenceException("Register the actual 3rd party signing service first.");
            }

            var token = await _thirdPartySignatureService.GenerateTokenAsync(userName, password);
            return !string.IsNullOrEmpty(token);
        }

        public async Task<EmbeddedSigning?> SendDocSignInvite(Customer recipient, string pdfUrl,
            bool isFirmNameLong)
        {
            if (_thirdPartySignatureService == null)
            {
                throw new NullReferenceException("Register the actual 3rd party signing service first.");
            }

            var signTextYIndent = 0;
            
            if (Path.GetFileNameWithoutExtension(pdfUrl) == DocumentNames.Proposal.ToString())
            {
                signTextYIndent = await _pdfParserService.GetPdfTextYIndent(pdfUrl, "Name:");
            }

            return await _thirdPartySignatureService.SendInviteToSignAsync(recipient, pdfUrl, signTextYIndent);
        }

        public async Task<EmbeddedSigning?> SendDocSignInviteAndEditPdf(Customer recipient, string pdfUrl)
        {
            if (_thirdPartySignatureService == null)
            {
                throw new NullReferenceException("Register the actual 3rd party signing service first.");
            }

            if (string.IsNullOrEmpty(recipient.Email))
            {
                throw new NullReferenceException(
                    $"Customer email should always have value in db in {nameof(SendDocSignInvite)}");
            }

            var editedPdfUrl = await _pdfWriterService.EditDirectDebitPdf(pdfUrl, recipient.Email);
            return await _thirdPartySignatureService.SendInviteToSignAsync(recipient, editedPdfUrl, 0);
        }

        public async Task<IEnumerable<EditDocumentPayload>> GetModifiedDocumentsAsync(string userName, string password)
        {
            const string signNowBaseUrl = "https://api-eval.signnow.com";
            const string fulfilledStatus = "fulfilled";

            var token = await _thirdPartySignatureService.GenerateTokenAsync(userName, password);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(new Uri($"{signNowBaseUrl}/user/documentsv2"));
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<EditDocumentPayload>>(content);

            if (result == null)
            {
                return new List<EditDocumentPayload>();
            }

            var fulfilledDocuments = result.Where(d => d.Invites.Any(
                i => i.Status.ToLower() == fulfilledStatus)).ToList();

            return fulfilledDocuments;
        }

        public async Task<DocumentK?> GetDocumentByIdAsync(string documentId)
        {
            if (_thirdPartySignatureService == null)
            {
                throw new NullReferenceException("Register the actual 3rd party signing service first.");
            }

            return await _thirdPartySignatureService.GetDocumentByIdAsync(documentId);
        }

        public async Task<string> GetDocumentDownloadLinkAsync(string documentId)
        {
            if (_thirdPartySignatureService == null)
            {
                throw new NullReferenceException("Register the actual 3rd party signing service first.");
            }

            return await _thirdPartySignatureService.GetDocumentDownloadLinkAsync(documentId);
        }

        public async Task<IEnumerable<FieldInviteK>> CreateEmbeddedInviteAsync(string documentId, string email,
            List<string> roleIds)
        {
            if (_thirdPartySignatureService == null)
            {
                throw new NullReferenceException("Register the actual 3rd party signing service first.");
            }

            return await _thirdPartySignatureService.CreateEmbeddedInviteAsync(documentId, email, roleIds);
        }

        public async Task<EmbeddedSigning> CreateEmbeddedInviteLinkAsync(string documentId, string fieldInviteId)
        {
            if (_thirdPartySignatureService == null)
            {
                throw new NullReferenceException("Register the actual 3rd party signing service first.");
            }

            return await _thirdPartySignatureService.CreateEmbeddedInviteLinkAsync(documentId, fieldInviteId);
        }
    }
}