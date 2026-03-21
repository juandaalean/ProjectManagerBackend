namespace Application.DTOs.Users;

/// <summary>
/// Request DTO for user registration.
/// </summary>
public record RegisterRequest(
    string Name,
    string Email,
    string Password
);
