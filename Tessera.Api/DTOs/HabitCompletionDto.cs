namespace Tessera.Api.DTOs;

/// <summary>
/// DTO representing a habit completion in API responses.
/// </summary>
public class HabitCompletionDto
{
    /// <summary>
    /// Unique identifier for the completion record.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Foreign key referencing the Habit.
    /// </summary>
    public required Guid HabitId { get; set; }

    /// <summary>
    /// Date on which the habit was completed.
    /// </summary>
    public required DateOnly Date { get; set; }
}