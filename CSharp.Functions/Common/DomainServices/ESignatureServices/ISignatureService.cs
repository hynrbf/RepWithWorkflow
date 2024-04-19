using Common.Entities;

namespace Common
{
    public interface ISignatureService
    {
        void Register(IThirdPartySigningService thirdPartyService, string callBackBaseUrl);

        Task<bool> AuthenticateAsync(string userName, string password);
        Task<EmbeddedSigning?> SendDocSignInvite(Customer recipient, string pdfUrl, bool isFirmNameLong);
        Task<EmbeddedSigning?> SendDocSignInviteAndEditPdf(Customer recipient, string pdfUrl);
        Task<IEnumerable<EditDocumentPayload>> GetModifiedDocumentsAsync(string userName, string password);
        Task<DocumentK?> GetDocumentByIdAsync(string documentId);
        Task<string> GetDocumentDownloadLinkAsync(string documentId);

        Task<IEnumerable<FieldInviteK>>
            CreateEmbeddedInviteAsync(string documentId, string email, List<string> roleIds);

        Task<EmbeddedSigning> CreateEmbeddedInviteLinkAsync(string documentId, string fieldInviteId);
    }
}