namespace Tessera.Api.DTOs;

/// <summary>
/// DTO for updating an existing habit.
/// </summary>
public class UpdateHabitDto
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