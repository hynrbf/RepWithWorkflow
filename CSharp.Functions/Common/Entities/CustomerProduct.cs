namespace Common.Entities
{
    public class CustomerProduct
    {
        public string? PageName { get; set; }

        public List<CategoryObject>? Categories { get; set; }
    }

    public class CategoryObject : CategoryProductObject
    {
        public List<ProductObject>? Products { get; set; }
    }

    public class ProductObject : CategoryProductObject
    {
    }

    public class CategoryProductObject
    {
        public string? Name { get; set; }
        public string? DisplayText { get; set; }
        public int? SortOrder { get; set; }
    }
}
