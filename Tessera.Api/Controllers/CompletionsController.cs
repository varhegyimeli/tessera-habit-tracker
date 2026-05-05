namespace Tessera.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Tessera.Api.DTOs;
using Tessera.Core.Interfaces;
using Tessera.Core.Models;

/// <summary>
/// API controller for managing habit completions.
/// </summary>
[ApiController]
[Route("api/habits/{habitId}/completions")]
public class CompletionsController : ControllerBase
{
    private readonly IHabitCompletionRepository _completionRepository;
    private readonly IHabitRepository _habitRepository;

    /// <summary>
    /// Initializes a new instance of the CompletionsController.
    /// </summary>
    /// <param name="completionRepository">The habit completion repository.</param>
    /// <param name="habitRepository">The habit repository.</param>
    public CompletionsController(IHabitCompletionRepository completionRepository, IHabitRepository habitRepository)
    {
        _completionRepository = completionRepository ?? throw new ArgumentNullException(nameof(completionRepository));
        _habitRepository = habitRepository ?? throw new ArgumentNullException(nameof(habitRepository));
    }

    /// <summary>
    /// Gets all completions for a specific habit.
    /// </summary>
    /// <param name="habitId">The habit's unique identifier.</param>
    /// <returns>A collection of completions for the habit.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HabitCompletionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByHabitId(Guid habitId)
    {
        var habit = await _habitRepository.GetByIdAsync(habitId);
        if (habit == null)
            return NotFound();

        var completions = await _completionRepository.GetByHabitIdAsync(habitId);
        var completionDtos = completions.Select(c => MapToDto(c));
        return Ok(completionDtos);
    }

    /// <summary>
    /// Creates a new completion record for a habit.
    /// </summary>
    /// <param name="habitId">The habit's unique identifier.</param>
    /// <param name="dto">The completion creation DTO.</param>
    /// <returns>201 Created if successful; 400 Bad Request if duplicate; 404 Not Found if habit doesn't exist.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(HabitCompletionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(Guid habitId, CreateCompletionDto dto)
    {
        var habit = await _habitRepository.GetByIdAsync(habitId);
        if (habit == null)
            return NotFound();

        // Check for duplicate completion on the same date
        var existing = await _completionRepository.GetByHabitIdAndDateAsync(habitId, dto.Date);
        if (existing != null)
            return BadRequest(new { message = "A completion already exists for this habit on the specified date." });

        var completion = new HabitCompletion
        {
            Id = Guid.NewGuid(),
            HabitId = habitId,
            Date = dto.Date
        };

        await _completionRepository.AddAsync(completion);
        return CreatedAtAction(nameof(GetByHabitId), new { habitId }, MapToDto(completion));
    }

    /// <summary>
    /// Deletes a completion record.
    /// </summary>
    /// <param name="habitId">The habit's unique identifier.</param>
    /// <param name="id">The completion's unique identifier.</param>
    /// <returns>204 No Content if successful; 404 Not Found if completion doesn't exist.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid habitId, Guid id)
    {
        var completions = await _completionRepository.GetByHabitIdAsync(habitId);
        var completion = completions.FirstOrDefault(c => c.Id == id);
        if (completion == null)
            return NotFound();

        await _completionRepository.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Maps a HabitCompletion model to a HabitCompletionDto.
    /// </summary>
    private static HabitCompletionDto MapToDto(HabitCompletion completion) => new()
    {
        Id = completion.Id,
        HabitId = completion.HabitId,
        Date = completion.Date
    };
}