using Common.Entities;
using Microsoft.AspNetCore.Http;

namespace Common
{
    public interface IWordToHtmlService
    {
        Task<HtmlContent> ConvertWordToHtmlThenSaveToDbAsync(IFormFile file, string fileExtension, string documentName);
        Task<string> ConvertSavedHtmlToWordAsync();
    }
}
