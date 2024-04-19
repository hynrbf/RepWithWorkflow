namespace Common
{
    public interface IFileDownloaderService
    {
        Task<Stream> DownloadAsStreamAsync(string path);
    }
}
