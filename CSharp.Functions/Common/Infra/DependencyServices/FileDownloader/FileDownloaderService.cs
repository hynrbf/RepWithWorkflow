namespace Common.Infra
{
    public class FileDownloaderService : IFileDownloaderService
    {
        private readonly IBlobContainerService _blobService;

        public FileDownloaderService(IBlobContainerService blobService)
        {
            _blobService = blobService;
        }

        public async Task<Stream> DownloadAsStreamAsync(string path)
        {
            var container = _blobService.BlobContainerClient;

            var blobClient = container.GetBlobClient(path);
            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"File: {path} does not exist.");
            }

            var blobDownloadInfo = await blobClient.DownloadAsync();
            return blobDownloadInfo.Value.Content;
        }
    }
}