namespace Tessera.Tests.Unit.Services;

using FluentAssertions;
using Moq;
using Tessera.Core.Interfaces;
using Tessera.Core.Models;
using Tessera.Core.Services;
using Xunit;

/// <summary>
/// Unit tests for the StatisticsService.
/// </summary>
public class StatisticsServiceTests
{
    private readonly Mock<IStreakCalculator> _mockStreakCalculator;
    private readonly StatisticsService _sut;

    public StatisticsServiceTests()
    {
        _mockStreakCalculator = new Mock<IStreakCalculator>();
        _sut = new StatisticsService(_mockStreakCalculator.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_NullStreakCalculator_ThrowsArgumentNullException()
    {
        // Arrange
        IStreakCalculator? streakCalculator = null;

        // Act
        var action = () => new StatisticsService(streakCalculator!);

        // Assert
        action.Should().Throw<ArgumentNullException>().WithParameterName(nameof(streakCalculator));
    }

    #endregion

    #region GetCurrentStreak Tests

    [Fact]
    public void GetCurrentStreak_DelegatesTo_CalculateCurrentStreak()
    {
        // Arrange
        var completions = new[] { new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.UtcNow) } };
        var expectedResult = 5;
        _mockStreakCalculator
            .Setup(x => x.CalculateCurrentStreak(It.IsAny<IEnumerable<HabitCompletion>>()))
            .Returns(expectedResult);

        // Act
        var result = _sut.GetCurrentStreak(completions);

        // Assert
        result.Should().Be(expectedResult);
        _mockStreakCalculator.Verify(x => x.CalculateCurrentStreak(completions), Times.Once);
    }

    #endregion

    #region GetLongestStreak Tests

    [Fact]
    public void GetLongestStreak_DelegatesTo_CalculateLongestStreak()
    {
        // Arrange
        var completions = new[] { new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.UtcNow) } };
        var expectedResult = 42;
        _mockStreakCalculator
            .Setup(x => x.CalculateLongestStreak(It.IsAny<IEnumerable<HabitCompletion>>()))
            .Returns(expectedResult);

        // Act
        var result = _sut.GetLongestStreak(completions);

        // Assert
        result.Should().Be(expectedResult);
        _mockStreakCalculator.Verify(x => x.CalculateLongestStreak(completions), Times.Once);
    }

    #endregion

    #region GetCompletionRate Tests

    [Fact]
    public void GetCompletionRate_AllDaysCompleted_ReturnsOne()
    {
        // Arrange
        var from = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-4);
        var to = DateOnly.FromDateTime(DateTime.UtcNow);
        var completions = new[]
        {
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = from },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = from.AddDays(1) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = from.AddDays(2) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = from.AddDays(3) },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = from.AddDays(4) }
        };

        // Act
        var result = _sut.GetCompletionRate(completions, from, to);

        // Assert
        result.Should().Be(1.0);
    }

    [Fact]
    public void GetCompletionRate_NoDaysCompleted_ReturnsZero()
    {
        // Arrange
        var from = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-10);
        var to = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-5);
        var completions = Enumerable.Empty<HabitCompletion>();

        // Act
        var result = _sut.GetCompletionRate(completions, from, to);

        // Assert
        result.Should().Be(0.0);
    }

    [Fact]
    public void GetCompletionRate_HalfDaysCompleted_ReturnsFifty()
    {
        // Arrange
        var from = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-3);
        var to = DateOnly.FromDateTime(DateTime.UtcNow);
        var completions = new[]
        {
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = from },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = from.AddDays(2) }
        };

        // Act
        var result = _sut.GetCompletionRate(completions, from, to);

        // Assert
        result.Should().Be(0.5);
    }

    [Fact]
    public void GetCompletionRate_SingleDayRangeDayCompleted_ReturnsOne()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var completions = new[]
        {
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = date }
        };

        // Act
        var result = _sut.GetCompletionRate(completions, date, date);

        // Assert
        result.Should().Be(1.0);
    }

    [Fact]
    public void GetCompletionRate_SingleDayRangeDayNotCompleted_ReturnsZero()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var completions = Enumerable.Empty<HabitCompletion>();

        // Act
        var result = _sut.GetCompletionRate(completions, date, date);

        // Assert
        result.Should().Be(0.0);
    }

    [Fact]
    public void GetCompletionRate_FromDateAfterToDate_ThrowsArgumentException()
    {
        // Arrange
        var from = DateOnly.FromDateTime(DateTime.UtcNow);
        var to = from.AddDays(-1);
        var completions = Enumerable.Empty<HabitCompletion>();

        // Act
        var action = () => _sut.GetCompletionRate(completions, from, to);

        // Assert
        action.Should().Throw<ArgumentException>().WithParameterName(nameof(from));
    }

    [Fact]
    public void GetCompletionRate_NullCompletions_ThrowsArgumentNullException()
    {
        // Arrange
        var from = DateOnly.FromDateTime(DateTime.UtcNow);
        var to = from.AddDays(5);
        IEnumerable<HabitCompletion>? completions = null;

        // Act
        var action = () => _sut.GetCompletionRate(completions!, from, to);

        // Assert
        action.Should().Throw<ArgumentNullException>().WithParameterName(nameof(completions));
    }

    [Fact]
    public void GetCompletionRate_CompletionsOutsideDateRange_NotIncludedInCalculation()
    {
        // Arrange
        var from = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-3);
        var to = DateOnly.FromDateTime(DateTime.UtcNow);
        var completions = new[]
        {
            // Outside range (before)
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = from.AddDays(-1) },
            // Inside range
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = from },
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = from.AddDays(1) },
            // Outside range (after)
            new HabitCompletion { Id = Guid.NewGuid(), HabitId = Guid.NewGuid(), Date = to.AddDays(1) }
        };

        // Act
        var result = _sut.GetCompletionRate(completions, from, to);

        // Assert
        // Total days in range: 4 (from to from+3)
        // Completions in range: 2
        // Expected: 2/4 = 0.5
        result.Should().Be(0.5);
    }

    #endregion
}