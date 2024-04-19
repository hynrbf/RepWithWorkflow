using System.Globalization;
using Common.Entities;
using CsvHelper;
using CsvHelper.Configuration;

namespace Common.Infra
{
    public class CsvLookupService : ICsvLookupService
    {
        private readonly IFileDownloaderService _fileDownloaderService;

        public CsvLookupService(IFileDownloaderService fileDownloaderService)
        {
            _fileDownloaderService = fileDownloaderService;
        }

        public async Task<List<CryptoName>> GetCryptoNamesAsync(string exception, string lookupFilename)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            var csvFileStream = await _fileDownloaderService.DownloadAsStreamAsync(lookupFilename);
            using var reader = new StreamReader(csvFileStream);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<CryptoName>();
            var cryptoNames = string.IsNullOrEmpty(exception)
                ? records.ToList()
                : records.Where(cn =>
                    cn.Exception.Equals(exception, StringComparison.InvariantCultureIgnoreCase)).ToList();

            return cryptoNames;
        }

        public async Task<List<T>> GetCsvDataAsync<T>(string lookupFilename)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            var csvFileStream = await _fileDownloaderService.DownloadAsStreamAsync(lookupFilename);
            using var reader = new StreamReader(csvFileStream);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<T>().ToList();

            return records;
        }
    }
}