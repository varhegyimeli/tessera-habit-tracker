namespace Tessera.Tests.Integration.RestApi;

using FluentAssertions;
using RestSharp;
using System.Net;
using Tessera.Api.DTOs;
using Xunit;

/// <summary>
/// REST API tests for the Habits endpoints using RestSharp.
/// These tests require Tessera.Api to be running locally on port 7184.
/// </summary>
public class HabitsApiTests : RestSharpApiTests
{
    [Fact]
    public async Task GetAllHabits_ReturnsSuccessStatusCode()
    {
        // Arrange
        var request = new RestRequest("/api/habits", Method.Get);

        // Act
        var response = await Client.ExecuteAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task CreateHabit_ValidData_ReturnsCreatedHabit()
    {
        // Arrange
        var createDto = new CreateHabitDto
        {
            Name = "Morning Run",
            Color = "#FF5722",
            Description = "Daily morning running habit"
        };

        var request = new RestRequest("/api/habits", Method.Post)
            .AddJsonBody(createDto);

        // Act
        var response = await Client.ExecuteAsync<HabitDto>(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.IsSuccessful.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data!.Name.Should().Be("Morning Run");
        response.Data.Color.Should().Be("#FF5722");
        response.Data.Description.Should().Be("Daily morning running habit");
        response.Data.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetHabitById_ExistingId_ReturnsHabit()
    {
        // Arrange - Create a habit first
        var createDto = new CreateHabitDto
        {
            Name = "Evening Walk",
            Color = "#4CAF50",
            Description = "Evening walk for relaxation"
        };

        var createRequest = new RestRequest("/api/habits", Method.Post)
            .AddJsonBody(createDto);
        var createResponse = await Client.ExecuteAsync<HabitDto>(createRequest);
        var createdHabitId = createResponse.Data!.Id;

        // Act - Get the habit by ID
        var getRequest = new RestRequest($"/api/habits/{createdHabitId}", Method.Get);
        var getResponse = await Client.ExecuteAsync<HabitDto>(getRequest);

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getResponse.IsSuccessful.Should().BeTrue();
        getResponse.Data.Should().NotBeNull();
        getResponse.Data!.Id.Should().Be(createdHabitId);
        getResponse.Data.Name.Should().Be("Evening Walk");
    }

    [Fact]
    public async Task DeleteHabit_ExistingId_ReturnsNoContent()
    {
        // Arrange - Create a habit first
        var createDto = new CreateHabitDto
        {
            Name = "Meditation",
            Color = "#9C27B0",
            Description = "Daily meditation"
        };

        var createRequest = new RestRequest("/api/habits", Method.Post)
            .AddJsonBody(createDto);
        var createResponse = await Client.ExecuteAsync<HabitDto>(createRequest);
        var createdHabitId = createResponse.Data!.Id;

        // Act - Delete the habit
        var deleteRequest = new RestRequest($"/api/habits/{createdHabitId}", Method.Delete);
        var deleteResponse = await Client.ExecuteAsync(deleteRequest);

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        deleteResponse.IsSuccessful.Should().BeTrue();

        // Verify it's deleted
        var getRequest = new RestRequest($"/api/habits/{createdHabitId}", Method.Get);
        var getResponse = await Client.ExecuteAsync(getRequest);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}