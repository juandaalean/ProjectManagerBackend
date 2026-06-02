namespace Application.DTOs.Sprints;
using Domain.Enum;

/// <summary>
/// Request DTO for updating an existing sprint.
/// </summary>
public record UpdateSprintRequest(
    string? Name,
    string? Goal,
    DateTime? StartDate,
    DateTime? EndDate,
    SprintState? State = null
);
