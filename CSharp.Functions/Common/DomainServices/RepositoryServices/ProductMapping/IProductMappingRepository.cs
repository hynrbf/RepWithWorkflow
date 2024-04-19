using Common.Entities;

namespace Common
{
    public interface IProductMappingRepository
    {
        Task<IEnumerable<ProductMapping>> GetProductMappingsAsync();
        Task<ProductMapping> SaveOrUpdateProductMappingAsync(ProductMapping productMapping);
    }
}