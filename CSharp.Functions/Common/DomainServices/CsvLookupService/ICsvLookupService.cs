using Common.Entities;

namespace Common
{
    /// <summary>
    /// Interface for providing lookup where values are saved in CSV file
    /// </summary>
    public interface ICsvLookupService
    {
        Task<List<CryptoName>> GetCryptoNamesAsync(string exception, string lookupFilename);

        Task<List<T>> GetCsvDataAsync<T>(string lookupFilename);
    }
}