using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Xunit;

namespace Tests
{
    public class IntegrationTest : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        protected readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;

        protected IntegrationTest(CustomWebApplicationFactory fixture)
        {
            _client = fixture.CreateClient();
            _serviceProvider = fixture.Services;
        }
        
        protected async Task AuthenticateAsync()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetJwtAsync());
        }
        
        private async Task<string> GetJwtAsync()
        {
            var response = await _client.PostAsJsonAsync("api/account/register", new RegisterDto()
            {
                Email = "user@test.com",
                Username = "test",
                DisplayName = "TestUser",
                Password = "Pa$$w0rd1"
            });
            
            var registrationResponse = await response.Content.ReadFromJsonAsync<UserDto>();

            return registrationResponse.Token;
        }

        public void Dispose()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();

            context.Database.EnsureDeleted();
        }
    }
}