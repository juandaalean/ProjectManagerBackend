using Domain.Entities;

namespace Domain.Abstractions;

/// <summary>
/// Repository interface for managing User entities.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user to add.</param>
    void Add(User user);

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
    /// Checks if an email is already used by another user.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the email exists, otherwise false.</returns>
    Task<bool> EmailExists(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user exists by their ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the user exists, otherwise false.</returns>
    Task<bool> Exists(Guid id, CancellationToken cancellationToken = default);
}