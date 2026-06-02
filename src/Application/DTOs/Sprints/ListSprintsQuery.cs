namespace Application.DTOs.Sprints;
using Domain.Enum;

/// <summary>
/// Query DTO for listing sprints within a project.
/// </summary>
public record ListSprintsQuery(
    string? SearchTerm = null,
    DateTime? StartDateFrom = null,
    DateTime? StartDateTo = null,
    SprintState? State = null
);
