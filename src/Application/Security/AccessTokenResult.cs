namespace Application.Security;

/// <summary>
/// Represents the generated access token output.
/// </summary>
/// <param name="AccessToken">The serialized JWT access token.</param>
/// <param name="ExpiresAtUtc">The token expiration date and time in UTC.</param>
/// <param name="TokenType">The token type, typically Bearer.</param>
public sealed record AccessTokenResult(
    string AccessToken,
    DateTime ExpiresAtUtc,
    string TokenType
);