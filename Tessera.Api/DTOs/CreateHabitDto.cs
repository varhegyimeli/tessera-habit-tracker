namespace Tessera.Api.DTOs;

/// <summary>
/// DTO for creating a new habit.
/// </summary>
public class CreateHabitDto
{
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
}