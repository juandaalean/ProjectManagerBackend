namespace Application.DTOs.Sprints;
using Domain.Enum;

/// <summary>
/// Response DTO for sprint information.
/// </summary>
public record SprintDto(
    Guid SprintId,
    Guid ProjectId,
    string Name,
    string? Goal,
    DateTime StartDate,
    DateTime EndDate,
    SprintState State = SprintState.Planned
);
