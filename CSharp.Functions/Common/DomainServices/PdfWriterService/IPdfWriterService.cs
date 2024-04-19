namespace Common
{
    public interface IPdfWriterService
    {
        Task<string> EditDirectDebitPdf(string pdfUrl, string email, string sComReferenceNumber = "11111111111111");
    }
}