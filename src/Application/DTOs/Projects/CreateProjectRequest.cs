namespace Application.DTOs.Projects;

/// <summary>
/// Request DTO for creating a new project.
/// </summary>
public record CreateProjectRequest(
    string Name,
    string? Description,
    DateTime StartDate,
    DateTime EndDate
);