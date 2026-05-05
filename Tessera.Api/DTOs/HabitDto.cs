namespace Tessera.Api.DTOs;

/// <summary>
/// DTO representing a habit in API responses.
/// </summary>
public class HabitDto
{
    /// <summary>
    /// Unique identifier for the habit.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Name of the habit.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Optional description providing additional context about the habit.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Hex color code for the habit (e.g., "#4CAF50").
    /// </summary>
    public required string Color { get; set; }

    /// <summary>
    /// DateTime when the habit was created.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
}