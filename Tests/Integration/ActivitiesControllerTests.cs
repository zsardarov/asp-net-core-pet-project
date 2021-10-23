using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain;
using FluentAssertions;
using Xunit;

namespace Tests.Integration
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

            var response = await Client.GetAsync("api/activities");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadFromJsonAsync<List<Activity>>()).Should().BeEmpty();
        }

        [Fact]
        public async Task Create_Activity_test()
        {
            await AuthenticateAsync();

            var id = Guid.NewGuid();

            var createActivityResponse = await Client.PostAsJsonAsync("api/activities", new Activity
            {
                Id = id,
                Title = "Test title",
                Description = "Sample",
                Category = "music",
                Date = DateTime.Now.AddDays(2),
                City = "New York",
                Venue = "Sample"
            });

            createActivityResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var getActivityResponse = await Client.GetAsync($"api/activities/{id}");

            getActivityResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            (await getActivityResponse.Content.ReadFromJsonAsync<Activity>())?.Id.Should().Be(id);
        }
    }
}