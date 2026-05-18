namespace Tessera.Tests.Unit;

using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Tessera.Core.Models;
using Tessera.Core.Services;

/// <summary>
/// Property-based tests using FsCheck for StreakCalculator and StatisticsService.
/// These tests verify invariants and properties that should hold across all inputs.
/// </summary>
public class FsCheckTests
{
    private readonly StreakCalculator _streakCalculator = new();
    private readonly StatisticsService _statisticsService;

    public FsCheckTests()
    {
        _statisticsService = new StatisticsService(_streakCalculator);
    }

    #region StreakCalculator.CalculateLongestStreak Properties

    /// <summary>
    /// Property: Empty input always returns 0.
    /// </summary>
    [Property]
    public void CalculateLongestStreak_EmptyInput_AlwaysReturnsZero()
    {
        // Arrange
        var completions = new List<HabitCompletion>();

        // Act
        var result = _streakCalculator.CalculateLongestStreak(completions);

        // Assert
        result.Should().Be(0);
    }

    /// <summary>
    /// Property: Result is always non-negative.
    /// </summary>
    [Property]
    public void CalculateLongestStreak_AnyInput_ResultIsNonNegative(int[] dayOffsets)
    {
        // Arrange
        var baseDate = new DateOnly(2024, 1, 1);
        var dates = dayOffsets
            .Where(o => o >= -365 && o <= 365) // Limit to reasonable range
            .Select(o => baseDate.AddDays(o))
            .ToList();
        var completions = ToCompletions(dates);

        // Act
        var result = _streakCalculator.CalculateLongestStreak(completions);

        // Assert
        result.Should().BeGreaterThanOrEqualTo(0);
    }

    /// <summary>
    /// Property: Result is never greater than the number of distinct dates.
    /// </summary>
    [Property]
    public void CalculateLongestStreak_AnyInput_ResultLessThanOrEqualDistinctDates(int[] dayOffsets)
    {
        // Arrange
        var baseDate = new DateOnly(2024, 1, 1);
        var dates = dayOffsets
            .Where(o => o >= -365 && o <= 365)
            .Select(o => baseDate.AddDays(o))
            .ToList();
        var completions = ToCompletions(dates);
        var distinctDateCount = dates.Distinct().Count();

        // Act
        var result = _streakCalculator.CalculateLongestStreak(completions);

        // Assert
        result.Should().BeLessThanOrEqualTo(distinctDateCount);
    }

    /// <summary>
    /// Property: Single date always returns 1.
    /// </summary>
    [Property]
    public void CalculateLongestStreak_SingleDate_ReturnsOne(int dayOffset)
    {
        // Arrange
        var baseDate = new DateOnly(2024, 1, 1);
        var offset = dayOffset % 365;
        var date = baseDate.AddDays(offset);
        var completions = ToCompletions(new[] { date });

        // Act
        var result = _streakCalculator.CalculateLongestStreak(completions);

        // Assert
        result.Should().Be(1);
    }

    /// <summary>
    /// Property: Adding duplicate dates does not change the result.
    /// </summary>
    [Property]
    public void CalculateLongestStreak_WithDuplicates_SameAsWithoutDuplicates(int[] dayOffsets)
    {
        // Arrange
        var baseDate = new DateOnly(2024, 1, 1);
        var dates = dayOffsets
            .Where(o => o >= -30 && o <= 30)
            .Select(o => baseDate.AddDays(o))
            .Distinct()
            .ToList();

        if (dates.Count == 0)
            dates.Add(baseDate);

        var completionsWithoutDuplicates = ToCompletions(dates);

        // Add some random duplicates
        var completionsWithDuplicates = new List<HabitCompletion>(completionsWithoutDuplicates);
        foreach (var date in dates.Take(Math.Max(1, dates.Count / 2)))
        {
            completionsWithDuplicates.Add(new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = date });
        }

        // Act
        var resultWithoutDuplicates = _streakCalculator.CalculateLongestStreak(completionsWithoutDuplicates);
        var resultWithDuplicates = _streakCalculator.CalculateLongestStreak(completionsWithDuplicates);

        // Assert
        resultWithDuplicates.Should().Be(resultWithoutDuplicates);
    }

    /// <summary>
    /// Property: Fully consecutive dates return length equal to the count of distinct dates.
    /// </summary>
    [Property]
    public void CalculateLongestStreak_FullyConsecutiveDates_EqualsDistinctCount(int count)
    {
        // Arrange
        var actualCount = Math.Abs(count % 30) + 1; // Keep between 1 and 30
        var baseDate = new DateOnly(2024, 1, 1);
        var dates = Enumerable.Range(0, actualCount)
            .Select(i => baseDate.AddDays(i))
            .ToList();
        var completions = ToCompletions(dates);

        // Act
        var result = _streakCalculator.CalculateLongestStreak(completions);

        // Assert
        result.Should().Be(actualCount);
    }

    #endregion

    #region StreakCalculator.CalculateCurrentStreak Properties

    /// <summary>
    /// Property: Empty input always returns 0.
    /// </summary>
    [Property]
    public void CalculateCurrentStreak_EmptyInput_AlwaysReturnsZero()
    {
        // Arrange
        var completions = new List<HabitCompletion>();

        // Act
        var result = _streakCalculator.CalculateCurrentStreak(completions);

        // Assert
        result.Should().Be(0);
    }

    /// <summary>
    /// Property: Result is always non-negative.
    /// </summary>
    [Property]
    public void CalculateCurrentStreak_AnyInput_ResultIsNonNegative(int[] dayOffsets)
    {
        // Arrange
        var baseDate = new DateOnly(2024, 1, 1);
        var dates = dayOffsets
            .Where(o => o >= -365 && o <= 365)
            .Select(o => baseDate.AddDays(o))
            .ToList();
        var completions = ToCompletions(dates);

        // Act
        var result = _streakCalculator.CalculateCurrentStreak(completions);

        // Assert
        result.Should().BeGreaterThanOrEqualTo(0);
    }

    /// <summary>
    /// Property: If no completion is today or yesterday, result is 0.
    /// </summary>
    [Property]
    public void CalculateCurrentStreak_AllPastDates_ReturnsZero(int[] dayOffsets)
    {
        // Arrange - Generate dates that are all at least 2 days in the past
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var earliestDate = today.AddDays(-2);
        var dates = dayOffsets
            .Where(o => o <= -2)
            .Select(o => today.AddDays(o))
            .ToList();

        if (dates.Count == 0)
            dates.Add(earliestDate);

        var completions = ToCompletions(dates);

        // Act
        var result = _streakCalculator.CalculateCurrentStreak(completions);

        // Assert
        result.Should().Be(0);
    }

    /// <summary>
    /// Property: Current streak is never greater than longest streak for the same input.
    /// </summary>
    [Property]
    public void CalculateCurrentStreak_NeverGreaterThanLongestStreak(int[] dayOffsets)
    {
        // Arrange
        var baseDate = new DateOnly(2024, 1, 1);
        var dates = dayOffsets
            .Where(o => o >= -365 && o <= 365)
            .Select(o => baseDate.AddDays(o))
            .ToList();
        var completions = ToCompletions(dates);

        // Act
        var currentStreak = _streakCalculator.CalculateCurrentStreak(completions);
        var longestStreak = _streakCalculator.CalculateLongestStreak(completions);

        // Assert
        currentStreak.Should().BeLessThanOrEqualTo(longestStreak);
    }

    #endregion

    #region StatisticsService.GetCompletionRate Properties

    /// <summary>
    /// Property: Result is always non-negative for valid input.
    /// </summary>
    [Property]
    public void GetCompletionRate_ValidInput_ResultIsNonNegative(int[] dayOffsets)
    {
        // Arrange
        var baseDate = new DateOnly(2024, 1, 1);
        var from = baseDate;
        var to = baseDate.AddDays(30);

        var dates = dayOffsets
            .Where(o => o >= 0 && o <= 30)
            .Select(o => from.AddDays(o))
            .ToList();
        var completions = ToCompletions(dates);

        // Act
        var result = _statisticsService.GetCompletionRate(completions, from, to);

        // Assert
        result.Should().BeGreaterThanOrEqualTo(0.0);
    }

    /// <summary>
    /// Property: Empty completions list always returns 0.0.
    /// </summary>
    [Property]
    public void GetCompletionRate_EmptyCompletions_ReturnsZero(int startOffset, int rangeLength)
    {
        // Arrange
        var baseDate = new DateOnly(2024, 1, 1);
        var offset = Math.Abs(startOffset % 365);
        var length = Math.Abs(rangeLength % 30) + 1;

        var from = baseDate.AddDays(offset);
        var to = from.AddDays(length);
        var completions = new List<HabitCompletion>();

        // Act
        var result = _statisticsService.GetCompletionRate(completions, from, to);

        // Assert
        result.Should().Be(0.0);
    }

    /// <summary>
    /// Property: Single day range with one matching completion returns 1.0.
    /// </summary>
    [Property]
    public void GetCompletionRate_SingleDayWithCompletion_ReturnsOne(int dayOffset)
    {
        // Arrange
        var baseDate = new DateOnly(2024, 1, 1);
        var offset = Math.Abs(dayOffset % 365);
        var date = baseDate.AddDays(offset);

        var completions = ToCompletions(new[] { date });

        // Act
        var result = _statisticsService.GetCompletionRate(completions, date, date);

        // Assert
        result.Should().Be(1.0);
    }

    /// <summary>
    /// Property: All completions outside date range returns 0.0.
    /// </summary>
    [Property]
    public void GetCompletionRate_CompletionsOutsideRange_ReturnsZero(int count, int offset)
    {
        // Arrange
        var baseDate = new DateOnly(2024, 1, 1);
        var from = baseDate;
        var to = baseDate.AddDays(10);

        // Generate completions before the range
        var actualCount = Math.Abs(count % 10) + 1;
        var negativeOffset = -(Math.Abs(offset % 100) + 20);
        var dates = Enumerable.Range(0, actualCount)
            .Select(i => from.AddDays(negativeOffset + i))
            .ToList();

        var completions = ToCompletions(dates);

        // Act
        var result = _statisticsService.GetCompletionRate(completions, from, to);

        // Assert
        result.Should().Be(0.0);
    }

    /// <summary>
    /// Property: from > to always throws ArgumentException.
    /// </summary>
    [Property]
    public void GetCompletionRate_FromGreaterThanTo_ThrowsArgumentException(int days)
    {
        // Arrange
        var baseDate = new DateOnly(2024, 1, 1);
        var from = baseDate.AddDays(10);
        var to = baseDate;
        var completions = new List<HabitCompletion>();

        // Act & Assert
        var action = () => _statisticsService.GetCompletionRate(completions, from, to);
        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Converts a list of DateOnly values to HabitCompletion objects.
    /// </summary>
    private static List<HabitCompletion> ToCompletions(IEnumerable<DateOnly> dates) =>
        dates.Select(d => new HabitCompletion
        {
            Id = Guid.NewGuid(),
            HabitId = Guid.NewGuid(),
            Date = d
        }).ToList();

    #endregion
}