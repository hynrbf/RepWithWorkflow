namespace Common
{
    public interface IFileUploaderService
    {
        Task<string> UploadFilesToAzureBlobStorageAsync(MemoryStream fileInMemory, string fileName,
            string fileExtension, string consultantName);
        Task<string> UploadCustomerFilesToAzureBlobStorageAsync(MemoryStream fileInMemory, string savePath, string fileName,
            string fileExtension);
    }
}
