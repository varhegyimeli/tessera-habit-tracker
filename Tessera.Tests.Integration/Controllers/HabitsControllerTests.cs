namespace Tessera.Tests.Integration.Controllers;

using FluentAssertions;
using System.Net;
using System.Text;
using System.Text.Json;
using Tessera.Api.DTOs;
using Tessera.Tests.Integration.Fixtures;
using Xunit;

/// <summary>
/// Integration tests for the HabitsController.
/// </summary>
public class HabitsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public HabitsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ResetDatabase();
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAllHabits_EmptyDatabase_ReturnsEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/habits");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var habits = JsonSerializer.Deserialize<List<HabitDto>>(content, _jsonOptions);
        habits.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllHabits_WithHabits_ReturnsAllHabits()
    {
        // Arrange
        var createDto1 = new CreateHabitDto { Name = "Morning Exercise", Color = "#4CAF50", Description = "Daily workout" };
        var createDto2 = new CreateHabitDto { Name = "Read", Color = "#2196F3", Description = null };

        await CreateHabitAsync(createDto1);
        await CreateHabitAsync(createDto2);

        // Act
        var response = await _client.GetAsync("/api/habits");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var habits = JsonSerializer.Deserialize<List<HabitDto>>(content, _jsonOptions);
        habits.Should().HaveCount(2);
        habits.Should().Contain(h => h.Name == "Morning Exercise");
        habits.Should().Contain(h => h.Name == "Read");
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetHabitById_ExistingId_ReturnsHabit()
    {
        // Arrange
        var createDto = new CreateHabitDto { Name = "Meditation", Color = "#9C27B0", Description = "Daily meditation" };
        var createdResponse = await CreateHabitAsync(createDto);
        var createdHabit = JsonSerializer.Deserialize<HabitDto>(createdResponse, _jsonOptions);

        // Act
        var response = await _client.GetAsync($"/api/habits/{createdHabit!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var habit = JsonSerializer.Deserialize<HabitDto>(content, _jsonOptions);
        habit!.Id.Should().Be(createdHabit.Id);
        habit.Name.Should().Be("Meditation");
        habit.Color.Should().Be("#9C27B0");
    }

    [Fact]
    public async Task GetHabitById_NonExistingId_Returns404()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/habits/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task CreateHabit_ValidData_Returns201WithCreatedHabit()
    {
        // Arrange
        var createDto = new CreateHabitDto { Name = "Yoga", Color = "#FF5722", Description = "Evening yoga" };

        // Act
        var response = await _client.PostAsync("/api/habits", new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        var habit = JsonSerializer.Deserialize<HabitDto>(content, _jsonOptions);
        habit!.Name.Should().Be("Yoga");
        habit.Color.Should().Be("#FF5722");
        habit.Description.Should().Be("Evening yoga");
        habit.Id.Should().NotBe(Guid.Empty);
        habit.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task CreateHabit_MissingName_Returns400()
    {
        // Arrange
        var invalidDto = new { Color = "#4CAF50" }; // Missing Name (required)

        // Act
        var response = await _client.PostAsync("/api/habits", new StringContent(
            JsonSerializer.Serialize(invalidDto),
            Encoding.UTF8,
            "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task UpdateHabit_ExistingId_Returns204()
    {
        // Arrange
        var createDto = new CreateHabitDto { Name = "Original Name", Color = "#4CAF50" };
        var createdResponse = await CreateHabitAsync(createDto);
        var createdHabit = JsonSerializer.Deserialize<HabitDto>(createdResponse, _jsonOptions);

        var updateDto = new UpdateHabitDto { Name = "Updated Name", Color = "#2196F3", Description = "Updated description" };

        // Act
        var response = await _client.PutAsync($"/api/habits/{createdHabit!.Id}", new StringContent(
            JsonSerializer.Serialize(updateDto),
            Encoding.UTF8,
            "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update
        var getResponse = await _client.GetAsync($"/api/habits/{createdHabit.Id}");
        var content = await getResponse.Content.ReadAsStringAsync();
        var updatedHabit = JsonSerializer.Deserialize<HabitDto>(content, _jsonOptions);
        updatedHabit!.Name.Should().Be("Updated Name");
        updatedHabit.Color.Should().Be("#2196F3");
        updatedHabit.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task UpdateHabit_NonExistingId_Returns404()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        var updateDto = new UpdateHabitDto { Name = "Updated", Color = "#4CAF50" };

        // Act
        var response = await _client.PutAsync($"/api/habits/{nonExistingId}", new StringContent(
            JsonSerializer.Serialize(updateDto),
            Encoding.UTF8,
            "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task DeleteHabit_ExistingId_Returns204()
    {
        // Arrange
        var createDto = new CreateHabitDto { Name = "To Delete", Color = "#4CAF50" };
        var createdResponse = await CreateHabitAsync(createDto);
        var createdHabit = JsonSerializer.Deserialize<HabitDto>(createdResponse, _jsonOptions);

        // Act
        var response = await _client.DeleteAsync($"/api/habits/{createdHabit!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/habits/{createdHabit.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteHabit_NonExistingId_Returns404()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/habits/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Helper Methods

    private async Task<string> CreateHabitAsync(CreateHabitDto dto)
    {
        var response = await _client.PostAsync("/api/habits", new StringContent(
            JsonSerializer.Serialize(dto),
            Encoding.UTF8,
            "application/json"));
        return await response.Content.ReadAsStringAsync();
    }

    #endregion
}