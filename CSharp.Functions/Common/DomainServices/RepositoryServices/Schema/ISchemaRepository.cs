using Common.Entities;

namespace Common
{
    [Obsolete]
    public interface ISchemaRepository
    {
        Task<bool> InitializeSchemasAsync();
        Task<SchemaModel> GetSchemaAsync(string formNameKey);
        Task<List<SchemaModel>> GetAllSchemaAsync();
        Task<SchemaModel> AddOrUpdateSchemaAsync(SchemaModel schemaModel);
    }
}
