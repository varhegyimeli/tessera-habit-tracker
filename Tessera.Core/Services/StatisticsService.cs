namespace Tessera.Core.Services;

using Tessera.Core.Interfaces;
using Tessera.Core.Models;

/// <summary>
/// Service for calculating habit statistics.
/// </summary>
public class StatisticsService : IStatisticsService
{
    private readonly IStreakCalculator _streakCalculator;

    /// <summary>
    /// Initializes a new instance of the StatisticsService.
    /// </summary>
    /// <param name="streakCalculator">The streak calculator service.</param>
    /// <exception cref="ArgumentNullException">Thrown when streakCalculator is null.</exception>
    public StatisticsService(IStreakCalculator streakCalculator)
    {
        _streakCalculator = streakCalculator ?? throw new ArgumentNullException(nameof(streakCalculator));
    }

    /// <summary>
    /// Gets the current streak for a habit.
    /// </summary>
    /// <param name="completions">The collection of completions for the habit.</param>
    /// <returns>The length of the current streak in days.</returns>
    public int GetCurrentStreak(IEnumerable<HabitCompletion> completions)
    {
        return _streakCalculator.CalculateCurrentStreak(completions);
    }

    /// <summary>
    /// Gets the longest streak ever recorded for a habit.
    /// </summary>
    /// <param name="completions">The collection of completions for the habit.</param>
    /// <returns>The length of the longest streak in days.</returns>
    public int GetLongestStreak(IEnumerable<HabitCompletion> completions)
    {
        return _streakCalculator.CalculateLongestStreak(completions);
    }

    /// <summary>
    /// Calculates the completion rate for a habit within a date range.
    /// </summary>
    /// <param name="completions">The collection of completions for the habit.</param>
    /// <param name="from">The start date (inclusive).</param>
    /// <param name="to">The end date (inclusive).</param>
    /// <returns>The completion rate as a percentage (0.0 to 100.0).</returns>
    /// <exception cref="ArgumentException">Thrown when 'from' is after 'to'.</exception>
    public double GetCompletionRate(IEnumerable<HabitCompletion> completions, DateOnly from, DateOnly to)
    {
        if (from > to)
            throw new ArgumentException("'from' date must not be after 'to' date.", nameof(from));

        if (completions == null)
            throw new ArgumentNullException(nameof(completions));

        // Calculate total days in range (inclusive)
        int totalDays = (to.DayNumber - from.DayNumber) + 1;

        // Count completions within the date range
        var completionsInRange = completions
            .Where(c => c.Date >= from && c.Date <= to)
            .Count();

        if (totalDays == 0)
            return 0.0;

        return (completionsInRange / (double)totalDays) * 100.0;
    }
}