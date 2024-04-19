using Azure.Storage.Blobs;

namespace Common.Infra
{
    public class BlobContainerService : IBlobContainerService
    {
        private BlobContainerClient? _blobContainerClient;

        public BlobContainerClient BlobContainerClient
        {
            get
            {
                if (_blobContainerClient == null)
                {
                    throw new NullReferenceException(
                        $"You must call {nameof(Register)} first in your constructor when using {nameof(IBlobContainerService)}");
                }

                return _blobContainerClient;
            }
        }

        [Obsolete("Please use Register(connectionString, containerName)")]
        public void Register()
        {
            var connectionString = Helpers.GetEnvironmentVariable("AzureStorageConnectionString");
            var containerName = Helpers.GetEnvironmentVariable("BlobStorageContainerName");
            var blobServiceClient = new BlobServiceClient(connectionString);
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }

        public void Register(string connectionString, string containerName)
        {
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(containerName))
            {
                throw new NullReferenceException($"Both connection string and container name should have value");
            }

            var blobServiceClient = new BlobServiceClient(connectionString);
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }
    }
}