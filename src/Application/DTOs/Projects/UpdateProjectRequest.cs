namespace Application.DTOs.Projects;
using Domain.Enum;

/// <summary>
/// Request DTO for updating an existing project.
/// </summary>
public record UpdateProjectRequest(
    string? Name,
    string? Description,
    DateTime? StartDate,
    DateTime? EndDate,
    ProjectState? State = null
);