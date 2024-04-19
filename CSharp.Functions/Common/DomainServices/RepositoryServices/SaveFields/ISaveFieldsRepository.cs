using Common.Entities;

namespace Common
{
    public interface ISaveFieldsRepository
    {
        Task<EditDocumentPayload> SaveFieldsAsync(EditDocumentPayload editDocumentPayload);
        Task<bool> GetSaveFieldsByIdAsync(string id);
    }
}
