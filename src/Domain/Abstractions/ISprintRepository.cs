using Domain.Entities;

namespace Domain.Abstractions;

/// <summary>
/// Repository interface for managing Sprint entities.
/// </summary>
public interface ISprintRepository
{
    /// <summary>
    /// Adds a new sprint to the repository.
    /// </summary>
    /// <param name="sprint">The sprint to add.</param>
    void Add(Sprint sprint);

    /// <summary>
    /// Retrieves a sprint by its ID.
    /// </summary>
    /// <param name="id">The sprint ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The sprint if found, otherwise null.</returns>
    Task<Sprint?> GetById(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sprint by its ID including its associated tasks.
    /// </summary>
    /// <param name="id">The sprint ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The sprint with its tasks if found, otherwise null.</returns>
    Task<Sprint?> GetByIdWithTasks(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of sprints for a specific project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="filter">Optional list filter contract.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of sprints.</returns>
    Task<IEnumerable<Sprint>> ListByProject(
        Guid projectId,
        SprintListFilter? filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all sprints for a project including their associated tasks.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of sprints with their tasks.</returns>
    Task<IEnumerable<Sprint>> ListByProjectWithTasks(
        Guid projectId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sprint.
    /// </summary>
    /// <param name="sprint">The sprint to update.</param>
    void Update(Sprint sprint);

    /// <summary>
    /// Deletes a sprint from the repository.
    /// </summary>
    /// <param name="sprint">The sprint to delete.</param>
    void Delete(Sprint sprint);
}
