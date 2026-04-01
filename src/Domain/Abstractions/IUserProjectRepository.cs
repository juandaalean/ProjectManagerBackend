using Domain.Entities;

namespace Domain.Abstractions;

/// <summary>
/// Repository interface for managing UserProject relationships.
/// </summary>
public interface IUserProjectRepository
{
    /// <summary>
    /// Adds a member to a project.
    /// </summary>
    /// <param name="userProject">The user-project relationship to add.</param>
    void AddMember(UserProject userProject);

    /// <summary>
    /// Removes a member from a project.
    /// </summary>
    /// <param name="userProject">The user-project relationship to remove.</param>
    void RemoveMember(UserProject userProject);

    /// <summary>
    /// Retrieves the membership of a user in a project.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="projectId">The project ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user-project relationship if found, otherwise null.</returns>
    Task<UserProject?> GetMembership(Guid userId, Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of members for a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of user-project relationships.</returns>
    Task<IEnumerable<UserProject>> ListMembers(Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all project memberships for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of user-project relationships.</returns>
    Task<IEnumerable<UserProject>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}