using Common.Entities;

namespace Common
{
    [Obsolete]
    public interface ISchemaAnswerRepository
    {
        Task<bool> InitializeSchemaAnswersAsync();
        Task<List<SchemaAnswer>> GetSchemaAnswersAsync();
        Task<SchemaAnswer> AddOrUpdateSchemaAnswerAsync(SchemaAnswer schemaAnswer);
        Task<bool> DeleteSchemaAnswerByIdAsync(string id);
    }
}
