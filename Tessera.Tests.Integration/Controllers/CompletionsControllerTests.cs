namespace Tessera.Tests.Integration.Controllers;

using FluentAssertions;
using System.Net;
using System.Text;
using System.Text.Json;
using Tessera.Api.DTOs;
using Tessera.Tests.Integration.Fixtures;
using Xunit;

/// <summary>
/// Integration tests for the CompletionsController.
/// </summary>
public class CompletionsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public CompletionsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ResetDatabase();
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    #region GetByHabitId Tests

    [Fact]
    public async Task GetCompletions_ExistingHabit_ReturnsCompletions()
    {
        // Arrange
        var habitId = await CreateHabitAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var yesterday = today.AddDays(-1);

        await CreateCompletionAsync(habitId, today);
        await CreateCompletionAsync(habitId, yesterday);

        // Act
        var response = await _client.GetAsync($"/api/habits/{habitId}/completions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var completions = JsonSerializer.Deserialize<List<HabitCompletionDto>>(content, _jsonOptions);
        completions.Should().HaveCount(2);
        completions.Should().Contain(c => c.Date == today);
        completions.Should().Contain(c => c.Date == yesterday);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task CreateCompletion_ValidData_Returns201()
    {
        // Arrange
        var habitId = await CreateHabitAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var createDto = new CreateCompletionDto { Date = today };

        // Act
        var response = await _client.PostAsync($"/api/habits/{habitId}/completions", new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        var completion = JsonSerializer.Deserialize<HabitCompletionDto>(content, _jsonOptions);
        completion!.HabitId.Should().Be(habitId);
        completion.Date.Should().Be(today);
    }

    [Fact]
    public async Task CreateCompletion_DuplicateDate_Returns400()
    {
        // Arrange
        var habitId = await CreateHabitAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var createDto = new CreateCompletionDto { Date = today };

        // Create first completion
        await _client.PostAsync($"/api/habits/{habitId}/completions", new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json"));

        // Act - Try to create duplicate
        var response = await _client.PostAsync($"/api/habits/{habitId}/completions", new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task DeleteCompletion_ExistingId_Returns204()
    {
        // Arrange
        var habitId = await CreateHabitAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var completionResponse = await CreateCompletionAsync(habitId, today);
        var completion = JsonSerializer.Deserialize<HabitCompletionDto>(completionResponse, _jsonOptions);

        // Act
        var response = await _client.DeleteAsync($"/api/habits/{habitId}/completions/{completion!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/habits/{habitId}/completions");
        var content = await getResponse.Content.ReadAsStringAsync();
        var completions = JsonSerializer.Deserialize<List<HabitCompletionDto>>(content, _jsonOptions);
        completions.Should().BeEmpty();
    }

    #endregion

    #region Helper Methods

    private async Task<Guid> CreateHabitAsync()
    {
        var createDto = new CreateHabitDto { Name = "Test Habit", Color = "#4CAF50" };
        var response = await _client.PostAsync("/api/habits", new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json"));
        var content = await response.Content.ReadAsStringAsync();
        var habit = JsonSerializer.Deserialize<HabitDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return habit!.Id;
    }

    private async Task<string> CreateCompletionAsync(Guid habitId, DateOnly date)
    {
        var createDto = new CreateCompletionDto { Date = date };
        var response = await _client.PostAsync($"/api/habits/{habitId}/completions", new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json"));
        return await response.Content.ReadAsStringAsync();
    }

    #endregion
}