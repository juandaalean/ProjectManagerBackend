namespace Application.DTOs.Projects;

/// <summary>
/// Response DTO for project information.
/// </summary>
public record ProjectDto(
    Guid ProjectId,
    string Name,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    Guid OwnerId
);