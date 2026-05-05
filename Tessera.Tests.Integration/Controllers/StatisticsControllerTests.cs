namespace Tessera.Tests.Integration.Controllers;

using FluentAssertions;
using System.Net;
using System.Text;
using System.Text.Json;
using Tessera.Api.DTOs;
using Tessera.Tests.Integration.Fixtures;
using Xunit;

/// <summary>
/// Integration tests for the StatisticsController.
/// </summary>
public class StatisticsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public StatisticsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ResetDatabase();
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    [Fact]
    public async Task GetStatistics_ExistingHabit_ReturnsStatisticsDto()
    {
        // Arrange
        var habitId = await CreateHabitAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Create a 3-day streak ending today
        await CreateCompletionAsync(habitId, today);
        await CreateCompletionAsync(habitId, today.AddDays(-1));
        await CreateCompletionAsync(habitId, today.AddDays(-2));

        // Act
        var response = await _client.GetAsync($"/api/habits/{habitId}/statistics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var statistics = JsonSerializer.Deserialize<StatisticsDto>(content, _jsonOptions);

        statistics!.CurrentStreak.Should().Be(3);
        statistics.LongestStreak.Should().Be(3);
        statistics.CompletionRate.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetStatistics_NonExistingHabit_Returns404()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/habits/{nonExistingId}/statistics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

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