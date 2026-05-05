namespace Tessera.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using Tessera.Core.Interfaces;
using Tessera.Core.Models;

/// <summary>
/// Repository implementation for HabitCompletion data access operations.
/// </summary>
public class HabitCompletionRepository : IHabitCompletionRepository
{
    private readonly TesseraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the HabitCompletionRepository.
    /// </summary>
    /// <param name="context">The Tessera DbContext.</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    public HabitCompletionRepository(TesseraDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves all completions for a specific habit.
    /// </summary>
    /// <param name="habitId">The habit's unique identifier.</param>
    /// <returns>A collection of completions for the specified habit.</returns>
    public async Task<IEnumerable<HabitCompletion>> GetByHabitIdAsync(Guid habitId)
    {
        return await _context.Completions
            .Where(c => c.HabitId == habitId)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific completion for a habit on a given date.
    /// </summary>
    /// <param name="habitId">The habit's unique identifier.</param>
    /// <param name="date">The date of the completion.</param>
    /// <returns>The completion if found; otherwise null.</returns>
    public async Task<HabitCompletion?> GetByHabitIdAndDateAsync(Guid habitId, DateOnly date)
    {
        return await _context.Completions
            .Where(c => c.HabitId == habitId && c.Date == date)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Adds a new completion record.
    /// </summary>
    /// <param name="completion">The completion to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when completion is null.</exception>
    public async Task AddAsync(HabitCompletion completion)
    {
        if (completion == null)
            throw new ArgumentNullException(nameof(completion));

        _context.Completions.Add(completion);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a completion record.
    /// </summary>
    /// <param name="id">The completion's unique identifier.</param>
    public async Task DeleteAsync(Guid id)
    {
        var completion = await _context.Completions.FindAsync(id);
        if (completion != null)
        {
            _context.Completions.Remove(completion);
            await _context.SaveChangesAsync();
        }
    }
}