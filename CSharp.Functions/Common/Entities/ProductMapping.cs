using Newtonsoft.Json;

namespace Common.Entities
{
    public class ProductMapping
    {
        [JsonProperty("id")] public string? Id { get; set; }
        public string? ProductName { get; set; }
        public string? DisplayText { get; set; }
        public int? SortOrder { get; set; }
        public ProductTypeEnum? ProductType { get; set; }
        public string? CategoryName { get; set; }
        public string? Enabler { get; set; }
        public EnablerType EnablerType { get; set; }
    }

    public enum ProductTypeEnum
    {
        Product,
        Category
    }

    public enum EnablerType
    {
        Page,
        Permission
    }
}
