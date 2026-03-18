using Domain.Entities;

namespace Application.Security;

/// <summary>
/// Provides operations for issuing access tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for the specified user.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <returns>The generated access token result.</returns>
    AccessTokenResult GenerateAccessToken(User user);
}