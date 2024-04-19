using Common.Entities;

namespace Common
{
    public interface IHtmlRepository
    {
        Task<List<HtmlContent>> GetHtmlSourcesAsync();
        Task<HtmlContent> GetHtmlSourceAsync(string name, string version, string consultant);
        Task<HtmlContent> GetHtmlSourcesAsync(string name);
        Task<HtmlContent> UpdateHtmlSourceAsync(HtmlContent htmlContent);
    }
}
