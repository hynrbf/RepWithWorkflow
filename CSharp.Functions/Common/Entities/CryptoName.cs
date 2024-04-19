using CsvHelper.Configuration.Attributes;

namespace Common.Entities
{
    public class CryptoName
    {
        public string Name { get; set; } = "";
        public string? Symbol { get; set; }
        public string Exception { get; set; } = "";
        public string? Reason { get; set; }
        [Name("Warning Type")] public string? WarningType { get; set; }
    }
}