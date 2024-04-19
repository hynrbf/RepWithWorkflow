using Common.Entities;

namespace Common.Infra
{
    public class PdfService : IPdfService
    {
        private IThirdPartyPdfService _thirdPartyPdfService;

        public void Register(IThirdPartyPdfService thirdPartyService) 
            => _thirdPartyPdfService = thirdPartyService;

        public async Task<string> ConvertToPdfAsync(HtmlContent htmlContent) 
            => await _thirdPartyPdfService.GeneratePdfAsync(htmlContent);
    }
}
