namespace Common
{
    public interface IPdfParserService
    {        
        Task<int> GetPdfTextYIndent(string pdfUrl, string text);
    }
}