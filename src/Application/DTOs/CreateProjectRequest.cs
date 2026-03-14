namespace Application.DTOs;

/// <summary>
/// Request DTO for creating a new project.
/// </summary>
public record CreateProjectRequest(
    string Name,
    string? Description,
    DateTime StartDate,
    DateTime EndDate
);