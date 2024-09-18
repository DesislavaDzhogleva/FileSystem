using FileSoftware.Constants;
using FileSoftware.Contracts;
using FileSoftware.Models;

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

        public FileContentModel GetFile(string filePath, string fileName)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(FileListingMessages.FileNotFound, filePath);
            }

            var fileContent = File.ReadAllBytes(filePath);
            var contentType = "application/octet-stream";

            return new FileContentModel
            {
                Content = fileContent,
                ContentType = contentType,
                FileName = fileName
            };
        }
    }
}
