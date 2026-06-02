namespace Application.DTOs.Sprints;

/// <summary>
/// Request DTO for creating a new sprint within a project.
/// </summary>
public record CreateSprintRequest(
    string Name,
    string? Goal,
    DateTime StartDate,
    DateTime EndDate
);
