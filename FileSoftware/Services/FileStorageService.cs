using FileSoftware.Contracts;

namespace FileSoftware.Services
{
    public class FileStorageService : IFileStorageService
    {
        public async Task UploadFileAsync(IFormFile file, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
    }
}
