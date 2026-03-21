using Domain.Entities;

namespace Application.Security;

/// <summary>
/// Provides password hashing and verification operations for users.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain-text password for the specified user.
    /// </summary>
    /// <param name="user">The user associated with the password.</param>
    /// <param name="password">The plain-text password.</param>
    /// <returns>The hashed password.</returns>
    string HashPassword(User user, string password);

    /// <summary>
    /// Verifies a plain-text password against a stored hash.
    /// </summary>
    /// <param name="user">The user associated with the password.</param>
    /// <param name="hashedPassword">The stored password hash.</param>
    /// <param name="providedPassword">The provided plain-text password.</param>
    /// <returns>True if the password is valid; otherwise, false.</returns>
    bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword);
}