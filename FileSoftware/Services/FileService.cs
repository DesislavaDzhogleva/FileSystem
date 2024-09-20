using AutoMapper;
using FileSoftware.Constants;
using FileSoftware.Contracts;
using FileSoftware.Data.Contracts;
using FileSoftware.Data.Entities;
using FileSoftware.Models;
using FileSoftware.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace FileSoftware.Services
{
    public class FileService : IFileService
    {
        private readonly IRepository<FileEntity> _fileRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        public FileService(
            IRepository<FileEntity> fileRepository,
            IFileStorageService fileStorageService,
            IMapper mapper,
            ILoggerFactory loggerFactory)
        {
            _fileRepository = fileRepository;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<FileService>();
        }

        public async Task DeleteFileAsync(int id)
        {
            var file = await _fileRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (file == null)
            {
                throw new FileNotFoundException(FileListingMessages.FileNotFound);
            }

            _fileRepository.Delete(file);
            await _fileRepository.ExecuteInTransactionAsync(async () =>
            {
                await _fileRepository.SaveChangesAsync();
                await _fileStorageService.DeleteFileAsync(file.FilePath);
            });
        }

        public async Task<IEnumerable<FileDto>> ListFilesAsync()
        {
            var files = await _fileRepository
                .AllAsNoTracking()
                .ToListAsync();

            return files.Select(x => _mapper.Map<FileDto>(x)).ToList();
        }
        public async Task<FileContentModel> GetFileAsync(int id)
        {
            var file = await _fileRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (file == null)
            {
                throw new FileNotFoundException(FileListingMessages.FileNotFound);
            }

            var filePath = Path.Combine(_storagePath, 
                FileConstants.FileNameWithIdentifier(file.Name, file.Extension, file.UniqueIdentifier.ToString()));

            var fileResult = _fileStorageService.GetFile(filePath, 
                FileConstants.FileNameWithoutIdentifier(file.Name, file.Extension));

            return fileResult;
        }

        public async Task<FileUploadResponse> UploadFilesAsync(IFormFile[] files)
        {
            var response = FileUploadResponse.CommonResponse(FileUploadMessages.CommonSuccess);
            var successResponses = new List<FileUploadResponseItem>();
            var filesToUpload = new List<Task>();

            foreach (var file in files)
            {
                //Validate each file
                if (file.Length == 0)
                {
                    AddFailedUpload(response, FileUploadMessages.FileIsEmpty(file.FileName), file.FileName);
                    continue;
                }

                //Check if file exists
                var currentFileInfo = await FileAlreadyExistsAsync(file.FileName);
                if (currentFileInfo.FileAlreadyExists)
                {
                    AddFailedUpload(response, FileUploadMessages.FileAlreadyExists(currentFileInfo.Name, currentFileInfo.Extensions), file.FileName);
                    continue;
                }

                //Add entity to database
                var fileEntity = this.CreateFileEntity(currentFileInfo, file);
                await _fileRepository.AddAsync(fileEntity);

                //Add task to upload file
                var uploadTask = _fileStorageService.UploadFileAsync(file, fileEntity.FilePath);
                filesToUpload.Add(uploadTask);

                //Add success message for this file
                var itemResponse = FileUploadResponseItem
                    .UploadSuccess(FileUploadMessages.FileUploadedSuccessfully(currentFileInfo.Name, currentFileInfo.Extensions), file.FileName);
                successResponses.Add(itemResponse);
            }

            //Save changes at once and upload all files asynchronously
            await _fileRepository.ExecuteInTransactionAsync(async () =>
            {
                await _fileRepository.SaveChangesAsync();
                await Task.WhenAll(filesToUpload);
            });
           
            //Add messages about rows after all files are uploaded without exceptions
            response.FileUploads = response.FileUploads.Concat(successResponses).ToList();

            return response;
        }

        private FileEntity CreateFileEntity(FileInfoModel fileInfoModel, IFormFile file)
        {
            var uniqueIdentifier = Guid.NewGuid();
            var filePath = Path.Combine(_storagePath, $"{fileInfoModel.Name}_{uniqueIdentifier}.{fileInfoModel.Extensions}");

            return new FileEntity
            {
                Name = fileInfoModel.Name,
                Extension = fileInfoModel.Extensions,
                UniqueIdentifier = uniqueIdentifier,
                FilePath = filePath,
                Size = file.Length,
                UploadedOn = DateTime.UtcNow
            };
        }

        private void AddFailedUpload(FileUploadResponse response, string message, string fileName)
        {
            var uploadFail = FileUploadResponseItem.UploadFail(message, fileName);
            response.FileUploads.Add(uploadFail);
        }

        private async Task<FileInfoModel> FileAlreadyExistsAsync(string fileName)
        {
            (string name, string extension) = ExtractFileNameAndExtension(fileName);

            bool fileExists = await _fileRepository
                .AnyAsync(x => x.Name == name && x.Extension == extension);

            if (fileExists && fileName.Count(c => c == '.') > 1)
            {
                (name, extension) = ExtractFileNameAndExtension(name, extension);

                fileExists = await _fileRepository
                    .AnyAsync(x => x.Name == name && x.Extension == extension);
            }

            return new FileInfoModel(fileExists, name, extension);
        }

        private (string name, string extension) ExtractFileNameAndExtension(
            string fileName, string currentExtension = null)
        {
            var fileNameParts = fileName.Split('.');
            string name = string.Join(".", fileNameParts.Take(fileNameParts.Length - 1));
            string extension = fileNameParts.Last();

            if (!string.IsNullOrEmpty(currentExtension))
            {
                extension += $".{currentExtension}";
            }
            return (name, extension);
        }
    }
}
