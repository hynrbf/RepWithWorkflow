using Aspose.Pdf;
using Aspose.Pdf.Text;

namespace Common.Infra
{
    public class PdfParserService : AsposeLicensedServiceBase, IPdfParserService
    {
        public PdfParserService()
        {
            // Has issue with AsposeLicensedServiceBase, use this for now.
            //Do not remove, the license in base will not throw error but
            //it's not attaching well
            var license = new License();
            license.SetLicense("Aspose.Total.NET.lic");
        }

        public async Task<int> GetPdfTextYIndent(string pdfUrl, string text)
        {
            using HttpClient httpClient = new();

            var pdfBytes = await httpClient.GetByteArrayAsync(pdfUrl);
            using MemoryStream pdfStream = new(pdfBytes);

            Document pdfDocument = new(pdfStream);
            TextFragmentAbsorber textFragmentAbsorber = new(text);

            pdfDocument.Pages.Accept(textFragmentAbsorber);

            var textFragments = textFragmentAbsorber.TextFragments;

            var yIndent = textFragments.FirstOrDefault()?.Position.YIndent ?? 0;
            return Convert.ToInt32(yIndent);
        }
    }
}