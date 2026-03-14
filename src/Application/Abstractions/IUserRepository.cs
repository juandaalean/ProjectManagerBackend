using Domain.Entities;

namespace Application.Abstractions;

/// <summary>
/// Repository interface for managing User entities.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<User?> GetById(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<User?> GetByEmail(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user exists by their ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the user exists, otherwise false.</returns>
    Task<bool> Exists(Guid id, CancellationToken cancellationToken = default);
}