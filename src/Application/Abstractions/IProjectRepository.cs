using Domain.Entities;

namespace Application.Abstractions;

/// <summary>
/// Repository interface for managing Project entities.
/// </summary>
public interface IProjectRepository
{
    /// <summary>
    /// Adds a new project to the repository.
    /// </summary>
    /// <param name="project">The project to add.</param>
    void Add(Project project);

    /// <summary>
    /// Retrieves a project by its ID.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The project if found, otherwise null.</returns>
    Task<Project?> GetById(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of projects owned by a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of projects.</returns>
    Task<IEnumerable<Project>> ListByUser(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="project">The project to update.</param>
    void Update(Project project);

    /// <summary>
    /// Deletes a project from the repository.
    /// </summary>
    /// <param name="project">The project to delete.</param>
    void Delete(Project project);
}