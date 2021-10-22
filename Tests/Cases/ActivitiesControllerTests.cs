using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain;
using FluentAssertions;
using Xunit;

namespace Tests.Cases
{
    public class ActivitiesControllerTests : IntegrationTest
    {
        public ActivitiesControllerTests(CustomWebApplicationFactory fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Get_Empty_Activities_List_Test()
        {
            await AuthenticateAsync();

            var response = await _client.GetAsync("api/activities");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadFromJsonAsync<List<Activity>>()).Should().BeEmpty();
        }

        [Fact]
        public async Task Create_Activity_test()
        {
            await AuthenticateAsync();

            var createActivityResponse = await _client.PostAsJsonAsync("api/activities", new Activity
            {
                Id = Guid.NewGuid(),
                Title = "Test title",
                Description = "Sample",
                Category = "music",
                Date = DateTime.Now.AddDays(2),
                City = "New York",
                Venue = "Sample"
            });

            createActivityResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var getActivityResponse = await _client.GetAsync("api/activities");

            getActivityResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            (await getActivityResponse.Content.ReadFromJsonAsync<List<Activity>>()).Should().NotBeEmpty();
        }
    }
}