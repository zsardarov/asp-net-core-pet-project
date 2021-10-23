using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Tests.Integration
{
    public class HomeControllerTests : IntegrationTest
    {
        public HomeControllerTests(CustomWebApplicationFactory fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Home_Page_Test()
        {
            var response = await Client.GetAsync("api");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync()).Should().Be("API home route");
        }
    }
}