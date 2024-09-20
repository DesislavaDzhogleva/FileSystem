using FileSoftware.Models;

namespace FileSoftware.Contracts
{
    public interface IFileStorageService
    {
        Task UploadFileAsync(IFormFile file, string filePath);

        FileContentModel GetFile(string filePath, string fileName);

        Task<bool> DeleteFileAsync(string filePath);
    }
}
