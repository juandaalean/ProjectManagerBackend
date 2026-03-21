using Domain.Enum;

namespace Application.DTOs.Projects;

/// <summary>
/// Response DTO for project member information.
/// </summary>
public record ProjectMemberDto(
    Guid UserId,
    string UserName,
    string UserEmail,
    UserRol Role
);