namespace Application.DTOs.Projects;

/// <summary>
/// Query DTO for listing projects.
/// </summary>
public record ListProjectsQuery(
    string? SearchTerm = null,
    DateTime? StartDateFrom = null,
    DateTime? StartDateTo = null
);