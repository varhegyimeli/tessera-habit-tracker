namespace Tessera.Core.Interfaces;

using Tessera.Core.Models;

/// <summary>
/// Service interface for habit statistics calculations.
/// </summary>
public interface IStatisticsService
{
    /// <summary>
    /// Gets the current streak for a habit.
    /// </summary>
    /// <param name="completions">The collection of completions for the habit.</param>
    /// <returns>The length of the current streak in days.</returns>
    int GetCurrentStreak(IEnumerable<HabitCompletion> completions);

    /// <summary>
    /// Gets the longest streak ever recorded for a habit.
    /// </summary>
    /// <param name="completions">The collection of completions for the habit.</param>
    /// <returns>The length of the longest streak in days.</returns>
    int GetLongestStreak(IEnumerable<HabitCompletion> completions);

    /// <summary>
    /// Calculates the completion rate for a habit within a date range.
    /// </summary>
    /// <param name="completions">The collection of completions for the habit.</param>
    /// <param name="from">The start date (inclusive).</param>
    /// <param name="to">The end date (inclusive).</param>
    /// <returns>The completion rate as a percentage (0.0 to 100.0).</returns>
    double GetCompletionRate(IEnumerable<HabitCompletion> completions, DateOnly from, DateOnly to);
}