using Application.Security;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Security.Services;

/// <summary>
/// Password hasher implementation based on ASP.NET Core Identity.
/// </summary>
public class AspNetPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    /// <inheritdoc/>
    public string HashPassword(User user, string password)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        return _passwordHasher.HashPassword(user, password);
    }

    /// <inheritdoc/>
    public bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);
        ArgumentException.ThrowIfNullOrWhiteSpace(providedPassword);

        var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result != PasswordVerificationResult.Failed;
    }
}
