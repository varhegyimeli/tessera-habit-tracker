namespace Tessera.Core.Models;

/// <summary>
/// Represents a recorded completion of a habit on a specific date.
/// </summary>
public class HabitCompletion
{
    /// <summary>
    /// Unique identifier for the completion record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key referencing the Habit.
    /// </summary>
    public Guid HabitId { get; set; }

    /// <summary>
    /// Date on which the habit was completed (no time component).
    /// </summary>
    public DateOnly Date { get; set; }
}