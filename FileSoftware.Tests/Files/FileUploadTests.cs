using FileSoftware.Constants;
using FileSoftware.Tests.Files.Abstraction;
using FileSoftware.Tests.Files.Configurations;
using System.Net;
using Xunit;

namespace FileSoftware.Tests.Files
{
    public class FileUploadTests : BaseFileTest, IClassFixture<CustomWebApplicationFactory<Program>>
    {
        public FileUploadTests(CustomWebApplicationFactory<Program> factory)
             : base(factory)
        {
        }

        [Fact]
        public async Task UploadFiles_ShouldReturnSuccess_WhenAllDataIsValid()
        {
            var token = await GetCsrfTokenAsync();

            var multipartContent = new MultipartFormDataContent();
            AddFileToMultipartContent(multipartContent, $"{Guid.NewGuid()}.txt", "This is a test file");
            AddFileToMultipartContent(multipartContent, $"{Guid.NewGuid()}.txt", "This is another test file");

            _client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token);

            var response = await _client.PostAsync("/api/files/upload", multipartContent);

            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            Assert.Contains(FileUploadMessages.CommonSuccess, responseData);
        }

        [Fact]
        public async Task UploadFiles_ShouldReturnConflictResponse_WhenThereIsAnEmptyFile()
        {
            var token = await GetCsrfTokenAsync();

            var multipartContent = new MultipartFormDataContent();
            AddFileToMultipartContent(multipartContent, $"{Guid.NewGuid()}.txt", "This is a test file");
            AddFileToMultipartContent(multipartContent, $"{Guid.NewGuid()}.txt", string.Empty);

            _client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token);

            var response = await _client.PostAsync("/api/files/upload", multipartContent);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            var responseData = await response.Content.ReadAsStringAsync();
            Assert.Contains(FileUploadMessages.MixedSuccess, responseData);
        }

        [Fact]
        public async Task UploadFiles_ShouldReturnFailResponse_WhenCreatingFilesWithSameNames()
        {
            var response1 = await UploadFileAsync("filename.txt", "This is a test file");
            response1.EnsureSuccessStatusCode();

            // Upload second file with the same name
            var response2 = await UploadFileAsync("filename.txt", "This is a test file 2");

            // Assert that the second request results in a conflict
            Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
            var responseData = await response2.Content.ReadAsStringAsync();
            Assert.Contains(FileUploadMessages.CommonFail, responseData);
        }

        [Fact]
        public async Task UploadFiles_ShouldReturnFailure_WhenEmptyProvided()
        {
            var csrfToken = await GetCsrfTokenAsync();

            var content = new MultipartFormDataContent();
            var multipartContent = new MultipartFormDataContent();
            AddFileToMultipartContent(multipartContent, $"{Guid.NewGuid()}.txt", string.Empty);
            AddFileToMultipartContent(multipartContent, $"{Guid.NewGuid()}.txt", string.Empty);

            _client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", csrfToken);

            var response = await _client.PostAsync("/api/files/upload", multipartContent);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            var responseData = await response.Content.ReadAsStringAsync();
            Assert.Contains(FileUploadMessages.CommonFail, responseData);
        }
    }
}
