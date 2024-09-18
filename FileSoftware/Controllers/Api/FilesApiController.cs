using FileSoftware.Constants;
using FileSoftware.Contracts;
using FileSoftware.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FileSoftware.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesApiController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        public FilesApiController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles([FromForm] IFormFile[] files)
        {
            if (files == null || files.Length == 0)
            {
                return BadRequest(FileUploadResponse.CommonResponse(FileUploadMessages.NoFilesProvided));
            }

            try
            {
                var result = await _fileService.UploadFilesAsync(files);
                if (result.HasConflictResponse || result.HasFailResponse)
                {
                    result.Message = result.HasConflictResponse 
                        ? FileUploadMessages.MixedSuccess 
                        : FileUploadMessages.CommonFail;

                    return Conflict(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, FileUploadResponse.CommonResponse(ex.Message));
            }
        }

        [HttpGet("listFiles")]
        public async Task<IActionResult> ListFiles()
        {
            try
            {
                var files = await _fileService.ListFilesAsync();
                return Ok(files);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var fileContent = await _fileService.GetFileAsync(id);

                var file = File(fileContent.Content, fileContent.ContentType, fileContent.FileName);
                return file;
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
