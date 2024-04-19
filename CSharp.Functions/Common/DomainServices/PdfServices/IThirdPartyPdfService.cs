using Common.Entities;

namespace Common
{
    public interface IThirdPartyPdfService
    {
        Task<string> GeneratePdfAsync(HtmlContent htmlContent);
    }
}
