namespace Common.Entities
{
    public class ProductType
    {
        public string? Label { get; set; }
        public string? Value { get; set; }
        public List<ProductType>? Items { get; set; }
    }
}