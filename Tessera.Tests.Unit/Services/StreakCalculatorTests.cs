namespace Tessera.Tests.Unit.Services;

using FluentAssertions;
using Tessera.Core.Models;
using Tessera.Core.Services;
using Xunit;

/// <summary>
/// Unit tests for the StreakCalculator service.
/// </summary>
public class StreakCalculatorTests
{
    private readonly StreakCalculator _sut;

    public StreakCalculatorTests()
    {
        _sut = new StreakCalculator();
    }

    #region CalculateCurrentStreak Tests

    [Fact]
    public void CalculateCurrentStreak_EmptyCompletion_ReturnsZero()
    {
        // Arrange
        var completions = Enumerable.Empty<HabitCompletion>();

        // Act
        var result = _sut.CalculateCurrentStreak(completions);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void CalculateCurrentStreak_OnlyTodayCompleted_ReturnsOne()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var completions = new[]
        {
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = today }
        };

        // Act
        var result = _sut.CalculateCurrentStreak(completions);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void CalculateCurrentStreak_YesterdayAndTodayCompleted_ReturnsTwo()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var yesterday = today.AddDays(-1);
        var completions = new[]
        {
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = today },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = yesterday }
        };

        // Act
        var result = _sut.CalculateCurrentStreak(completions);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public void CalculateCurrentStreak_SeveralConsecutiveDaysEndingToday_ReturnsCorrectCount()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var completions = new[]
        {
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = today },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = today.AddDays(-1) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = today.AddDays(-2) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = today.AddDays(-3) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = today.AddDays(-4) }
        };

        // Act
        var result = _sut.CalculateCurrentStreak(completions);

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void CalculateCurrentStreak_SeveralConsecutiveDaysEndingYesterdayTodayNotCompleted_ReturnsCorrectCount()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var yesterday = today.AddDays(-1);
        var completions = new[]
        {
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = yesterday },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = yesterday.AddDays(-1) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = yesterday.AddDays(-2) }
        };

        // Act
        var result = _sut.CalculateCurrentStreak(completions);

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public void CalculateCurrentStreak_StreakBrokenBeforeToday_ReturnsOnlyDaysSinceLastBreak()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var completions = new[]
        {
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = today },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = today.AddDays(-1) },
            // Gap: today.AddDays(-2) is missing (streak broken here)
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = today.AddDays(-5) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = today.AddDays(-6) }
        };

        // Act
        var result = _sut.CalculateCurrentStreak(completions);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public void CalculateCurrentStreak_SingleCompletionNotTodayOrYesterday_ReturnsZero()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var completions = new[]
        {
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = today.AddDays(-10) }
        };

        // Act
        var result = _sut.CalculateCurrentStreak(completions);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void CalculateCurrentStreak_NullCompletions_ThrowsArgumentNullException()
    {
        // Arrange
        IEnumerable<HabitCompletion>? completions = null;

        // Act
        var action = () => _sut.CalculateCurrentStreak(completions!);

        // Assert
        action.Should().Throw<ArgumentNullException>().WithParameterName(nameof(completions));
    }

    #endregion

    #region CalculateLongestStreak Tests

    [Fact]
    public void CalculateLongestStreak_EmptyCompletion_ReturnsZero()
    {
        // Arrange
        var completions = Enumerable.Empty<HabitCompletion>();

        // Act
        var result = _sut.CalculateLongestStreak(completions);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void CalculateLongestStreak_SingleCompletion_ReturnsOne()
    {
        // Arrange
        var completions = new[]
        {
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.UtcNow) }
        };

        // Act
        var result = _sut.CalculateLongestStreak(completions);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void CalculateLongestStreak_AllDaysConsecutive_ReturnsTotalCount()
    {
        // Arrange
        var baseDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var completions = new[]
        {
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = baseDate },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = baseDate.AddDays(1) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = baseDate.AddDays(2) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = baseDate.AddDays(3) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = baseDate.AddDays(4) }
        };

        // Act
        var result = _sut.CalculateLongestStreak(completions);

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void CalculateLongestStreak_TwoSeparateStreaks_ReturnsTheLongerOne()
    {
        // Arrange
        var baseDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var completions = new[]
        {
            // First streak: 3 days
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = baseDate },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = baseDate.AddDays(1) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = baseDate.AddDays(2) },
            // Gap: baseDate.AddDays(3) is missing
            // Second streak: 5 days
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = baseDate.AddDays(4) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = baseDate.AddDays(5) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = baseDate.AddDays(6) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = baseDate.AddDays(7) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = baseDate.AddDays(8) }
        };

        // Act
        var result = _sut.CalculateLongestStreak(completions);

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void CalculateLongestStreak_DuplicateDatesInInput_CountedAsOneDay()
    {
        // Arrange
        var baseDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var habitId = Guid.NewGuid();
        var completions = new[]
        {
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = habitId, Date = baseDate },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = habitId, Date = baseDate },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = habitId, Date = baseDate.AddDays(1) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = habitId, Date = baseDate.AddDays(1) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = habitId, Date = baseDate.AddDays(2) }
        };

        // Act
        var result = _sut.CalculateLongestStreak(completions);

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public void CalculateLongestStreak_NullCompletions_ThrowsArgumentNullException()
    {
        // Arrange
        IEnumerable<HabitCompletion>? completions = null;

        // Act
        var action = () => _sut.CalculateLongestStreak(completions!);

        // Assert
        action.Should().Throw<ArgumentNullException>().WithParameterName(nameof(completions));
    }

    #endregion
}