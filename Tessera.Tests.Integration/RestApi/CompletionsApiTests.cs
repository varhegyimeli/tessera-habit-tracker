namespace Tessera.Tests.Integration.RestApi;

using FluentAssertions;
using RestSharp;
using System.Net;
using Tessera.Api.DTOs;
using Xunit;

/// <summary>
/// REST API tests for the Completions endpoints using RestSharp.
/// These tests require Tessera.Api to be running locally on port 7184.
/// </summary>
public class CompletionsApiTests : RestSharpApiTests
{
    [Fact]
    public async Task CreateCompletion_ValidData_ReturnsCreated()
    {
        // Arrange - Create a habit first
        var habitId = await CreateTestHabitAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var createDto = new CreateCompletionDto { Date = today };

        var request = new RestRequest($"/api/habits/{habitId}/completions", Method.Post)
            .AddJsonBody(createDto);

        // Act
        var response = await Client.ExecuteAsync<HabitCompletionDto>(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.IsSuccessful.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data!.HabitId.Should().Be(habitId);
        response.Data.Date.Should().Be(today);
        response.Data.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateCompletion_DuplicateDate_ReturnsBadRequest()
    {
        // Arrange - Create a habit first
        var habitId = await CreateTestHabitAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var createDto = new CreateCompletionDto { Date = today };

        // Create first completion
        var firstRequest = new RestRequest($"/api/habits/{habitId}/completions", Method.Post)
            .AddJsonBody(createDto);
        var firstResponse = await Client.ExecuteAsync<HabitCompletionDto>(firstRequest);
        firstResponse.IsSuccessful.Should().BeTrue();

        // Act - Try to create duplicate
        var secondRequest = new RestRequest($"/api/habits/{habitId}/completions", Method.Post)
            .AddJsonBody(createDto);
        var secondResponse = await Client.ExecuteAsync(secondRequest);

        // Assert
        secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        secondResponse.IsSuccessful.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteCompletion_ExistingId_ReturnsNoContent()
    {
        // Arrange - Create a habit and completion
        var habitId = await CreateTestHabitAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var createDto = new CreateCompletionDto { Date = today };
        var createRequest = new RestRequest($"/api/habits/{habitId}/completions", Method.Post)
            .AddJsonBody(createDto);
        var createResponse = await Client.ExecuteAsync<HabitCompletionDto>(createRequest);
        var completionId = createResponse.Data!.Id;

        // Act - Delete the completion
        var deleteRequest = new RestRequest($"/api/habits/{habitId}/completions/{completionId}", Method.Delete);
        var deleteResponse = await Client.ExecuteAsync(deleteRequest);

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        deleteResponse.IsSuccessful.Should().BeTrue();

        // Verify it's deleted by checking the completions list
        var getRequest = new RestRequest($"/api/habits/{habitId}/completions", Method.Get);
        var getResponse = await Client.ExecuteAsync<List<HabitCompletionDto>>(getRequest);
        getResponse.Data.Should().BeEmpty();
    }

    #region Helper Methods

    private async Task<Guid> CreateTestHabitAsync()
    {
        var createDto = new CreateHabitDto
        {
            Name = $"Test Habit {Guid.NewGuid()}",
            Color = "#4CAF50"
        };

        var request = new RestRequest("/api/habits", Method.Post)
            .AddJsonBody(createDto);

        var response = await Client.ExecuteAsync<HabitDto>(request);
        return response.Data!.Id;
    }

    #endregion
}