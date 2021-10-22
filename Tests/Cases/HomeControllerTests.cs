using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Tests.Cases
{
    public class HomeControllerTests : IntegrationTest
    {
        public HomeControllerTests(CustomWebApplicationFactory fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Home_Page_Test()
        {
            var response = await _client.GetAsync("api");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync()).Should().Be("API home route");
        }
    }
}