using FileSoftware.Models;
using FileSoftware.Tests.Files.Abstraction;
using FileSoftware.Tests.Files.Configurations;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FileSoftware.Tests.Files
{
    public class FileListTests : BaseFileTest, IClassFixture<CustomWebApplicationFactory<Program>>
    {
        public FileListTests(CustomWebApplicationFactory<Program> factory)
             : base(factory)
        {
        }


        [Fact]
        public async Task ListFiles_ShouldReturnListOfFiles()
        {
            var uploadResponse = await UploadFileAsync($"{Guid.NewGuid()}.txt", "This is a test file to be deleted");
            uploadResponse.EnsureSuccessStatusCode();

            var uploadResponse2 = await UploadFileAsync($"{Guid.NewGuid()}.txt", "This is a test file to be deleted");
            uploadResponse2.EnsureSuccessStatusCode();

            var token = await GetCsrfTokenAsync();
            _client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token);

            var response = await _client.GetAsync("/api/files/listFiles");

            response.EnsureSuccessStatusCode(); 

            var files = await response.Content.ReadFromJsonAsync<List<FileDto>>();

            files.Should().HaveCount(2);  
        }

        [Fact]
        public async Task ListFiles_ShouldReturnListOfOneFile_WhenOneIvalid()
        {
            var uploadResponse = await UploadFileAsync($"{Guid.NewGuid()}.txt", "This is a test file to be deleted");
            uploadResponse.EnsureSuccessStatusCode();

            var uploadResponse2 = await UploadFileAsync($"{Guid.NewGuid()}.txt", string.Empty);
            Assert.Equal(HttpStatusCode.Conflict, uploadResponse2.StatusCode);

            var token = await GetCsrfTokenAsync();
            _client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token);

            var response = await _client.GetAsync("/api/files/listFiles");

            response.EnsureSuccessStatusCode();
            var files = await response.Content.ReadFromJsonAsync<List<FileDto>>();

            files.Should().HaveCount(3);
        }
    }
}
