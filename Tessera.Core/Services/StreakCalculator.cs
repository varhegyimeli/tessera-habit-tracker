namespace Tessera.Core.Services;

using Tessera.Core.Interfaces;
using Tessera.Core.Models;

/// <summary>
/// Service for calculating streak information based on habit completions.
/// </summary>
public class StreakCalculator : IStreakCalculator
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
    public int CalculateCurrentStreak(IEnumerable<HabitCompletion> completions)
    {
        if (completions == null)
            throw new ArgumentNullException(nameof(completions));

        var sortedCompletions = completions
            .OrderByDescending(c => c.Date)
            .ToList();

        if (sortedCompletions.Count == 0)
            return 0;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = sortedCompletions.First().Date;

        // If the most recent completion is not today or yesterday, streak is broken
        if (startDate != today && startDate != today.AddDays(-1))
            return 0;

        // Count consecutive days backwards
        int streak = 1;
        DateOnly expectedDate = startDate.AddDays(-1);

        for (int i = 1; i < sortedCompletions.Count; i++)
        {
            if (sortedCompletions[i].Date == expectedDate)
            {
                streak++;
                expectedDate = expectedDate.AddDays(-1);
            }
            else if (sortedCompletions[i].Date < expectedDate)
            {
                // Gap found, streak ends
                break;
            }
        }

        return streak;
    }

    /// <summary>
    /// Calculates the longest streak ever recorded for a habit.
    /// </summary>
    /// <param name="completions">The collection of completions for the habit.</param>
    /// <returns>The length of the longest streak in days.</returns>
    public int CalculateLongestStreak(IEnumerable<HabitCompletion> completions)
    {
        if (completions == null)
            throw new ArgumentNullException(nameof(completions));

        // Deduplicate by grouping on Date and selecting distinct dates
        var uniqueDates = completions
            .Select(c => c.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        if (uniqueDates.Count == 0)
            return 0;

        int longestStreak = 1;
        int currentStreak = 1;

        for (int i = 1; i < uniqueDates.Count; i++)
        {
            // Check if this date is consecutive to the previous one
            if (uniqueDates[i] == uniqueDates[i - 1].AddDays(1))
            {
                currentStreak++;
                longestStreak = Math.Max(longestStreak, currentStreak);
            }
            else
            {
                // Gap found, reset current streak
                currentStreak = 1;
            }
        }

        return longestStreak;
    }
}