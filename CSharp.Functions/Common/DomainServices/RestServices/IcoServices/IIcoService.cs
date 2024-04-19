using Common.Entities;

namespace Common;

public interface IIcoService
{
    Task<IcoDetails> SearchDetailsAsync(IcoSearchInput companyName);

    Task<IcoDetails> GetDetailsByReferenceAsync(string reference);

    Task<string> SaveRegistrationCertificate(string reference);

    void Register(string azureStorageConnectionString, string blobStorageContainerName);
}