using Azure.Storage.Blobs;

namespace Common
{
    public interface IBlobContainerService
    {
        BlobContainerClient BlobContainerClient { get; }

        [Obsolete("Please use Register(connectionString, containerName)")]
        void Register();
        void Register(string connectionString, string containerName);
    }
}
