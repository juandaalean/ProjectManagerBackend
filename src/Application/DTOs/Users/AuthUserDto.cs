namespace Application.DTOs.Users;

/// <summary>
/// Basic user data returned with authentication responses.
/// </summary>
public record AuthUserDto(
    Guid UserId,
    string Name,
    string Email,
    string Rol
);
