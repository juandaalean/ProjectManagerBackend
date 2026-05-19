using Domain.Enum;

namespace Application.DTOs.Projects;

/// <summary>
/// Request DTO for updating a user's role in a project.
/// </summary>
public record UserRoleUpdateRequest(
    UserRol Role
);
