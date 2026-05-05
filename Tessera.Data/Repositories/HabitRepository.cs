namespace Tessera.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using Tessera.Core.Interfaces;
using Tessera.Core.Models;

/// <summary>
/// Repository implementation for Habit data access operations.
/// </summary>
public class HabitRepository : IHabitRepository
{
    private readonly TesseraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the HabitRepository.
    /// </summary>
    /// <param name="context">The Tessera DbContext.</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    public HabitRepository(TesseraDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves all habits.
    /// </summary>
    /// <returns>A collection of all habits.</returns>
    public async Task<IEnumerable<Habit>> GetAllAsync()
    {
        return await _context.Habits
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a habit by its unique identifier.
    /// </summary>
    /// <param name="id">The habit's unique identifier.</param>
    /// <returns>The habit if found; otherwise null.</returns>
    public async Task<Habit?> GetByIdAsync(Guid id)
    {
        return await _context.Habits
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    /// <summary>
    /// Adds a new habit to the repository.
    /// </summary>
    /// <param name="habit">The habit to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when habit is null.</exception>
    public async Task AddAsync(Habit habit)
    {
        if (habit == null)
            throw new ArgumentNullException(nameof(habit));

        _context.Habits.Add(habit);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing habit.
    /// </summary>
    /// <param name="habit">The habit with updated values.</param>
    /// <exception cref="ArgumentNullException">Thrown when habit is null.</exception>
    public async Task UpdateAsync(Habit habit)
    {
        if (habit == null)
            throw new ArgumentNullException(nameof(habit));

        _context.Habits.Update(habit);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a habit by its unique identifier.
    /// </summary>
    /// <param name="id">The habit's unique identifier.</param>
    public async Task DeleteAsync(Guid id)
    {
        var habit = await _context.Habits.FindAsync(id);
        if (habit != null)
        {
            _context.Habits.Remove(habit);
            await _context.SaveChangesAsync();
        }
    }
}