using Domain.Enum;

namespace Application.DTOs;

/// <summary>
/// Request DTO for adding a member to a project.
/// </summary>
public record AddProjectMemberRequest(
    Guid UserId,
    UserRol Role
);