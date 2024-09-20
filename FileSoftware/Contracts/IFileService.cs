using FileSoftware.Models;
using FileSoftware.Models.Responses;

namespace FileSoftware.Contracts
{
    public interface IFileService
    {
        Task<FileUploadResponse> UploadFilesAsync(IFormFile[] files);

        Task<IEnumerable<FileDto>> ListFilesAsync();

        Task<FileContentModel> GetFileAsync(int id);

        Task DeleteFileAsync(int id);
    }
}
