namespace Tessera.Core.Interfaces;

using Tessera.Core.Models;

/// <summary>
/// Repository interface for Habit aggregate operations.
/// </summary>
public interface IHabitRepository
{
    /// <summary>
    /// Retrieves all habits.
    /// </summary>
    /// <returns>A collection of all habits.</returns>
    Task<IEnumerable<Habit>> GetAllAsync();

    /// <summary>
    /// Retrieves a habit by its unique identifier.
    /// </summary>
    /// <param name="id">The habit's unique identifier.</param>
    /// <returns>The habit if found; otherwise null.</returns>
    Task<Habit?> GetByIdAsync(Guid id);

    /// <summary>
    /// Adds a new habit to the repository.
    /// </summary>
    /// <param name="habit">The habit to add.</param>
    Task AddAsync(Habit habit);

    /// <summary>
    /// Updates an existing habit.
    /// </summary>
    /// <param name="habit">The habit with updated values.</param>
    Task UpdateAsync(Habit habit);

    /// <summary>
    /// Deletes a habit by its unique identifier.
    /// </summary>
    /// <param name="id">The habit's unique identifier.</param>
    Task DeleteAsync(Guid id);
}