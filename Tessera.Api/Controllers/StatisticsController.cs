namespace Tessera.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Tessera.Api.DTOs;
using Tessera.Core.Interfaces;

/// <summary>
/// API controller for habit statistics.
/// </summary>
[ApiController]
[Route("api/habits/{habitId}/statistics")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;
    private readonly IHabitCompletionRepository _completionRepository;
    private readonly IHabitRepository _habitRepository;

    /// <summary>
    /// Initializes a new instance of the StatisticsController.
    /// </summary>
    /// <param name="statisticsService">The statistics service.</param>
    /// <param name="completionRepository">The habit completion repository.</param>
    /// <param name="habitRepository">The habit repository.</param>
    public StatisticsController(
        IStatisticsService statisticsService,
        IHabitCompletionRepository completionRepository,
        IHabitRepository habitRepository)
    {
        _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
        _completionRepository = completionRepository ?? throw new ArgumentNullException(nameof(completionRepository));
        _habitRepository = habitRepository ?? throw new ArgumentNullException(nameof(habitRepository));
    }

    /// <summary>
    /// Gets statistics for a specific habit.
    /// </summary>
    /// <param name="habitId">The habit's unique identifier.</param>
    /// <returns>The habit statistics.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(StatisticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatistics(Guid habitId)
    {
        var habit = await _habitRepository.GetByIdAsync(habitId);
        if (habit == null)
            return NotFound();

        var completions = await _completionRepository.GetByHabitIdAsync(habitId);

        var currentStreak = _statisticsService.GetCurrentStreak(completions);
        var longestStreak = _statisticsService.GetLongestStreak(completions);

        var to = DateOnly.FromDateTime(DateTime.UtcNow);
        var from = to.AddDays(-29); // Last 30 days (inclusive)

        var completionRate = _statisticsService.GetCompletionRate(completions, from, to);

        var dto = new StatisticsDto
        {
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak,
            CompletionRate = completionRate
        };

        return Ok(dto);
    }
}