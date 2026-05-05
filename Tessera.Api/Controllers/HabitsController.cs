namespace Tessera.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Tessera.Api.DTOs;
using Tessera.Core.Interfaces;
using Tessera.Core.Models;

/// <summary>
/// API controller for managing habits.
/// </summary>
[ApiController]
[Route("api/habits")]
public class HabitsController : ControllerBase
{
    private readonly IHabitRepository _habitRepository;

    /// <summary>
    /// Initializes a new instance of the HabitsController.
    /// </summary>
    /// <param name="habitRepository">The habit repository.</param>
    public HabitsController(IHabitRepository habitRepository)
    {
        _habitRepository = habitRepository ?? throw new ArgumentNullException(nameof(habitRepository));
    }

    /// <summary>
    /// Gets all habits.
    /// </summary>
    /// <returns>A collection of all habits.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HabitDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var habits = await _habitRepository.GetAllAsync();
        var habitDtos = habits.Select(h => MapToDto(h));
        return Ok(habitDtos);
    }

    /// <summary>
    /// Gets a habit by its unique identifier.
    /// </summary>
    /// <param name="id">The habit's unique identifier.</param>
    /// <returns>The habit if found; otherwise 404 Not Found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HabitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var habit = await _habitRepository.GetByIdAsync(id);
        if (habit == null)
            return NotFound();

        return Ok(MapToDto(habit));
    }

    /// <summary>
    /// Creates a new habit.
    /// </summary>
    /// <param name="dto">The habit creation DTO.</param>
    /// <returns>201 Created with the created habit.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(HabitDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateHabitDto dto)
    {
        var habit = new Habit
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Color = dto.Color,
            CreatedAt = DateTime.UtcNow
        };

        await _habitRepository.AddAsync(habit);
        return CreatedAtAction(nameof(GetById), new { id = habit.Id }, MapToDto(habit));
    }

    /// <summary>
    /// Updates an existing habit.
    /// </summary>
    /// <param name="id">The habit's unique identifier.</param>
    /// <param name="dto">The habit update DTO.</param>
    /// <returns>204 No Content if successful; 404 Not Found if habit doesn't exist.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateHabitDto dto)
    {
        var habit = await _habitRepository.GetByIdAsync(id);
        if (habit == null)
            return NotFound();

        habit.Name = dto.Name;
        habit.Description = dto.Description;
        habit.Color = dto.Color;

        await _habitRepository.UpdateAsync(habit);
        return NoContent();
    }

    /// <summary>
    /// Deletes a habit.
    /// </summary>
    /// <param name="id">The habit's unique identifier.</param>
    /// <returns>204 No Content if successful; 404 Not Found if habit doesn't exist.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var habit = await _habitRepository.GetByIdAsync(id);
        if (habit == null)
            return NotFound();

        await _habitRepository.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Maps a Habit model to a HabitDto.
    /// </summary>
    private static HabitDto MapToDto(Habit habit) => new()
    {
        Id = habit.Id,
        Name = habit.Name,
        Description = habit.Description,
        Color = habit.Color,
        CreatedAt = habit.CreatedAt
    };
}