using Common.Entities;

namespace Common
{
    public interface IPdfService
    {
        void Register(IThirdPartyPdfService thirdPartyService);
        Task<string> ConvertToPdfAsync(HtmlContent htmlContent);
    }
}
