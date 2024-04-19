using Azure.Storage.Blobs;
using Common.Entities;
using Microsoft.Extensions.Logging;

namespace Common
{
    public interface IDocGeneratorService
    {
        Task GenerateDocumentAsync(Customer customer, string documentName, ILogger log,
            BlobContainerClient blobContainerClient, string rootDirectory);
    }
}