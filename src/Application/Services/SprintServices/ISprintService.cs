using Application.DTOs.Sprints;

namespace Application.Services.SprintServices;

/// <summary>
/// Service interface for sprint-related operations.
/// </summary>
public interface ISprintService
{
    /// <summary>
    /// Creates a new sprint inside a project.
    /// </summary>
    /// <param name="projectId">The project ID where the sprint belongs.</param>
    /// <param name="request">The create sprint request.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created sprint DTO.</returns>
    Task<SprintDto> CreateSprintAsync(Guid projectId, CreateSprintRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sprint by ID inside a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sprintId">The sprint ID.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The sprint DTO if found.</returns>
    Task<SprintDto> GetSprintAsync(Guid projectId, Guid sprintId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists sprints belonging to a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="query">Optional query parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of sprint DTOs.</returns>
    Task<IEnumerable<SprintDto>> ListSprintsInProjectAsync(Guid projectId, Guid actorUserId, ListSprintsQuery? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the sprint board for a project: each sprint with its tasks plus the backlog (tasks without a sprint).
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of sprint groups with their tasks. The backlog (no sprint) is included as the first entry with a null sprint.</returns>
    Task<IEnumerable<SprintWithTasksDto>> GetSprintBoardAsync(Guid projectId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sprint together with its tasks.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sprintId">The sprint ID.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The sprint and its tasks.</returns>
    Task<SprintWithTasksDto> GetSprintWithTasksAsync(Guid projectId, Guid sprintId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sprint.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sprintId">The sprint ID.</param>
    /// <param name="request">The update sprint request.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated sprint DTO.</returns>
    Task<SprintDto> UpdateSprintAsync(Guid projectId, Guid sprintId, UpdateSprintRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sprint. Tasks belonging to it are not removed; they are moved to the project backlog.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sprintId">The sprint ID.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task DeleteSprintAsync(Guid projectId, Guid sprintId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns an existing task to a sprint.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sprintId">The sprint ID.</param>
    /// <param name="taskItemId">The task ID to assign.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AssignTaskToSprintAsync(Guid projectId, Guid sprintId, Guid taskItemId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a task from its sprint (moves it to the backlog).
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sprintId">The sprint ID the task currently belongs to.</param>
    /// <param name="taskItemId">The task ID to remove.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task RemoveTaskFromSprintAsync(Guid projectId, Guid sprintId, Guid taskItemId, Guid actorUserId, CancellationToken cancellationToken = default);
}
