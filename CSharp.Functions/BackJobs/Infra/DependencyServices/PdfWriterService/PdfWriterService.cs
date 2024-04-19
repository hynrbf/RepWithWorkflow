using System.IO;
using System.Net.Http;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using System.Threading.Tasks;
using Common;

namespace BackJobs.Infra
{
    public class PdfWriterService : IPdfWriterService
    {
        private readonly IBlobContainerService _blobContainerClientService;

        public PdfWriterService(IBlobContainerService blobContainerClientService)
        {
            _blobContainerClientService = blobContainerClientService;
        }

        public async Task<string> EditDirectDebitPdf(string pdfUrl, string email,
            string sComReferenceNumber = "11111111111111")
        {
            var containerClient = _blobContainerClientService.BlobContainerClient;
            using HttpClient httpClient = new();
            var pdfBytes = await httpClient.GetByteArrayAsync(pdfUrl);

            using MemoryStream pdfStream = new(pdfBytes);
            Document pdfDocument = new(pdfStream);
            var outputFileLocation = $"ConversionProcess/pdf.files.scom/{email}/DirectDebitMandate.pdf";
            var baseUrl = containerClient.Uri;
            var outputFullUrlPath = $"{baseUrl}/{outputFileLocation}";
            var spacedComReferenceNumber = string.Join("   ", sComReferenceNumber.ToCharArray());
            var textFragment = new TextFragment(spacedComReferenceNumber)
            {
                Position = new Position(109, 262)
            };
            pdfDocument.Pages[1].Paragraphs.Add(textFragment);

            await using var memoryStream = new MemoryStream();
            pdfDocument.Save(memoryStream, SaveFormat.Pdf);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var blobClient = containerClient.GetBlobClient(outputFileLocation);
            await blobClient.UploadAsync(memoryStream, true);

            return outputFullUrlPath;
        }
    }
}