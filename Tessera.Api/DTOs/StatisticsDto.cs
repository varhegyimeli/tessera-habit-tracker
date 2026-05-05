namespace Tessera.Api.DTOs;

/// <summary>
/// DTO representing habit statistics in API responses.
/// </summary>
public class StatisticsDto
{
    /// <summary>
    /// Current consecutive streak length in days.
    /// </summary>
    public required int CurrentStreak { get; set; }

    /// <summary>
    /// Longest consecutive streak length ever recorded in days.
    /// </summary>
    public required int LongestStreak { get; set; }

    /// <summary>
    /// Completion rate as a percentage (0.0 to 100.0) for the last 30 days.
    /// </summary>
    public required double CompletionRate { get; set; }
}