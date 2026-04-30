namespace Tessera.Core.Interfaces;

using Tessera.Core.Models;

/// <summary>
/// Repository interface for HabitCompletion aggregate operations.
/// </summary>
public interface IHabitCompletionRepository
{
    /// <summary>
    /// Retrieves all completions for a specific habit.
    /// </summary>
    /// <param name="habitId">The habit's unique identifier.</param>
    /// <returns>A collection of completions for the specified habit.</returns>
    Task<IEnumerable<HabitCompletion>> GetByHabitIdAsync(Guid habitId);

    /// <summary>
    /// Retrieves a specific completion for a habit on a given date.
    /// </summary>
    /// <param name="habitId">The habit's unique identifier.</param>
    /// <param name="date">The date of the completion.</param>
    /// <returns>The completion if found; otherwise null.</returns>
    Task<HabitCompletion?> GetByHabitIdAndDateAsync(Guid habitId, DateOnly date);

    /// <summary>
    /// Adds a new completion record.
    /// </summary>
    /// <param name="completion">The completion to add.</param>
    Task AddAsync(HabitCompletion completion);

    /// <summary>
    /// Deletes a completion record.
    /// </summary>
    /// <param name="id">The completion's unique identifier.</param>
    Task DeleteAsync(Guid id);
}