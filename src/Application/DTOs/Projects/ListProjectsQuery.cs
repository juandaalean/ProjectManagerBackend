namespace Application.DTOs.Projects;
using Domain.Enum;

/// <summary>
/// Query DTO for listing projects.
/// </summary>
public record ListProjectsQuery(
    string? SearchTerm = null,
    DateTime? StartDateFrom = null,
    DateTime? StartDateTo = null,
    ProjectState? State = null
);