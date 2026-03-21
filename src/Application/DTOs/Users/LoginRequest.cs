namespace Application.DTOs.Users;

/// <summary>
/// Request DTO for user login.
/// </summary>
public record LoginRequest(
    string Email,
    string Password
);
