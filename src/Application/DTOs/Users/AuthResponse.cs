namespace Application.DTOs.Users;

/// <summary>
/// Response DTO for authentication operations.
/// </summary>
public record AuthResponse(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAtUtc,
    AuthUserDto User
);
