namespace FileSoftware.Contracts
{
    public interface IFileStorageService
    {
        Task UploadFileAsync(IFormFile file, string filePath);
    }
}
