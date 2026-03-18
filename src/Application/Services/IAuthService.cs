using Application.DTOs.Users;

namespace Application.Services;

/// <summary>
/// Service interface for authentication operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user and returns an access token.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The authentication response with access token and user information.</returns>
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user and returns an access token.
    /// </summary>
    /// <param name="request">The login request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The authentication response with access token and user information.</returns>
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}