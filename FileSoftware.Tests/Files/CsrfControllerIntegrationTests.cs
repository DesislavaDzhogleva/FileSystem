using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using Xunit;

namespace FileSoftware.Tests.Files
{
    public class CsrfControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CsrfControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetCsrfToken_ReturnsToken()
        {
            var response = await _client.GetAsync("/api/csrf/token");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var csrfResponse = JsonDocument.Parse(content);

            Assert.True(csrfResponse.RootElement.TryGetProperty("token", out var tokenElement));
            Assert.False(string.IsNullOrEmpty(tokenElement.GetString()), "CSRF token should not be null or empty.");
        }
    }
}
