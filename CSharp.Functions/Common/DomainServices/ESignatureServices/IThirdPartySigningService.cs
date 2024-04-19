using Common.Entities;

namespace Common;

public interface IThirdPartySigningService
{
    Task<string> GenerateTokenAsync(string userName, string password);
    Task<EmbeddedSigning?> SendInviteToSignAsync(Customer recipient, string pdfUrl, int signTextYIndent);
    Task<DocumentK?> GetDocumentByIdAsync(string documentId);
    Task<string> GetDocumentDownloadLinkAsync(string documentId);
    Task<IEnumerable<FieldInviteK>> CreateEmbeddedInviteAsync(string documentId, string email, List<string> roleId);
    Task<EmbeddedSigning> CreateEmbeddedInviteLinkAsync(string documentId, string fieldInviteId);
    void Register(string callBackBaseUrl);
}