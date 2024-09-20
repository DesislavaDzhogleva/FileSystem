using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FileSoftware.Tests.Files.Configurations;

namespace FileSoftware.Tests.Files.Abstraction
{
    public abstract class BaseFileTest
    {
        protected readonly HttpClient _client;

        public BaseFileTest(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        protected async Task<HttpResponseMessage> UploadFileAsync(string fileName, string fileContent)
        {
            var token = await GetCsrfTokenAsync();
            _client.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");
            _client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token);

            var multipartContent = new MultipartFormDataContent();
            AddFileToMultipartContent(multipartContent, fileName, fileContent);

            var response = await _client.PostAsync("/api/files/upload", multipartContent);
            return response;
        }

        protected void AddFileToMultipartContent(MultipartFormDataContent multipartContent, string fileName, string fileContent)
        {
            var fileContentByteArray = new ByteArrayContent(Encoding.UTF8.GetBytes(fileContent));
            fileContentByteArray.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "files",
                FileName = fileName
            };
            fileContentByteArray.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            multipartContent.Add(fileContentByteArray);
        }

        protected virtual async Task<string> GetCsrfTokenAsync()
        {
            var csrfResponse = await _client.GetAsync("/api/csrf/token");
            csrfResponse.EnsureSuccessStatusCode();
            var csrfToken = await csrfResponse.Content.ReadAsStringAsync();
            return ExtractToken(csrfToken);
        }

        protected string ExtractToken(string csrfResponse)
        {
            var token = csrfResponse.Split(':')[1].Trim('"', '}');
            return token;
        }

        protected async Task<List<int>> GetSuccessfulFileIdsFromUploadResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(content);

            var fileUploads = jsonDocument.RootElement.GetProperty("fileUploads").EnumerateArray();
            var successfulFileIds = new List<int>();

            foreach (var fileUpload in fileUploads)
            {
                if (fileUpload.GetProperty("status").GetBoolean()) 
                {
                    var fileId = fileUpload.GetProperty("id").GetInt32(); 
                    successfulFileIds.Add(fileId);
                }
            }

            return successfulFileIds; 
        }
    }
}
