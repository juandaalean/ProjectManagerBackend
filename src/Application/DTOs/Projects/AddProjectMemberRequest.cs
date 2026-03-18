using Domain.Enum;

namespace Application.DTOs.Projects;

/// <summary>
/// Request DTO for adding a member to a project.
/// </summary>
public record AddProjectMemberRequest(
    Guid UserId,
    UserRol Role
);