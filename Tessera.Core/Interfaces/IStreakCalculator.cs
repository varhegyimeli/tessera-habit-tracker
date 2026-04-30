namespace Tessera.Core.Interfaces;

using Tessera.Core.Models;

/// <summary>
/// Service interface for calculating streak information.
/// </summary>
public interface IStreakCalculator
{
    /// <summary>
    /// Calculates the current streak for a habit.
    /// 
    /// The current streak is the number of consecutive days ending today or yesterday
    /// that have a completion. If today is completed, the streak includes today.
    /// If today is not completed, the streak ends yesterday.
    /// </summary>
    /// <param name="completions">The collection of completions for the habit.</param>
    /// <returns>The length of the current streak in days.</returns>
    int CalculateCurrentStreak(IEnumerable<HabitCompletion> completions);

    /// <summary>
    /// Calculates the longest streak ever recorded for a habit.
    /// </summary>
    /// <param name="completions">The collection of completions for the habit.</param>
    /// <returns>The length of the longest streak in days.</returns>
    int CalculateLongestStreak(IEnumerable<HabitCompletion> completions);
}