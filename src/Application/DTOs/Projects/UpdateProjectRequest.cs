namespace Application.DTOs.Projects;

/// <summary>
/// Request DTO for updating an existing project.
/// </summary>
public record UpdateProjectRequest(
    string? Name,
    string? Description,
    DateTime? StartDate,
    DateTime? EndDate
);