namespace Tessera.Api.DTOs;

/// <summary>
/// DTO for creating a habit completion record.
/// </summary>
public class CreateCompletionDto
{
    /// <summary>
    /// Date on which the habit was completed.
    /// </summary>
    public required DateOnly Date { get; set; }
}