using FileSoftware.Models.Responses;

namespace FileSoftware.Contracts
{
    public interface IFileService
    {
        Task<FileUploadResponse> UploadFilesAsync(IFormFile[] files);
    }
}
