using Application.DTOs;

namespace Application.Services;

/// <summary>
/// Service interface for project-related operations.
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="request">The create project request.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created project DTO.</returns>
    Task<ProjectDto> CreateProjectAsync(CreateProjectRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a project by ID.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The project DTO if found.</returns>
    Task<ProjectDto> GetProjectAsync(Guid projectId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists projects for a user.
    /// </summary>
    /// <param name="actorUserId">The ID of the user.</param>
    /// <param name="query">Optional query parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of project DTOs.</returns>
    Task<IEnumerable<ProjectDto>> ListProjectsForUserAsync(Guid actorUserId, ListProjectsQuery? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="request">The update project request.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated project DTO.</returns>
    Task<ProjectDto> UpdateProjectAsync(Guid projectId, UpdateProjectRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task DeleteProjectAsync(Guid projectId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a member to a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="request">The add member request.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AddMemberAsync(Guid projectId, AddProjectMemberRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a member from a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="userId">The user ID to remove.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task RemoveMemberAsync(Guid projectId, Guid userId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the role of a member in a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="role">The new role.</param>
    /// <param name="actorUserId">The ID of the user performing the action.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task UpdateMemberRoleAsync(Guid projectId, Guid userId, Domain.Enum.UserRol role, Guid actorUserId, CancellationToken cancellationToken = default);
}