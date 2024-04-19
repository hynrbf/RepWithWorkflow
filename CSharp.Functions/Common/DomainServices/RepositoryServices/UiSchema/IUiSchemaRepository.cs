using Common.Entities;

namespace Common
{
    public interface IUiSchemaRepository
    {
        Task<bool> InitializeUiSchemasAsync();
        Task<UiSchemaModel> GetUiSchemaAsync(string formNameKey);
        Task<List<UiSchemaModel>> GetAllUiSchemaAsync();
        Task<UiSchemaModel> AddOrUpdateUiSchemaAsync(UiSchemaModel uiSchemaModel);
    }
}
