namespace Tessera.Core.Models;

/// <summary>
/// Represents a habit that a user wants to track.
/// </summary>
public class Habit
{
    /// <summary>
    /// Unique identifier for the habit.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the habit.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Optional description providing additional context about the habit.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// DateTime when the habit was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Hex color code for the habit (e.g., "#4CAF50").
    /// </summary>
    public required string Color { get; set; }
}