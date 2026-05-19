using Application.DTOs.Projects;
using Application.Exceptions;
using Application.Services;
using Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProjectManagerAPI.Controllers;

/// <summary>
/// Controller for managing projects.
/// </summary>
[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController(IProjectService projectService) : ControllerBase
{
    /// <summary>
    /// Gets all projects for the current user.
    /// </summary>
    /// <param name="query">Optional query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of projects.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects([FromQuery] ListProjectsQuery? query, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var projects = await projectService.ListProjectsForUserAsync(actorUserId, query, cancellationToken);
        return Ok(projects);
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="request">The create project request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created project.</returns>
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectRequest request, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var project = await projectService.CreateProjectAsync(request, actorUserId, cancellationToken);
        return CreatedAtAction(nameof(GetProjects), new { id = project.ProjectId }, project);
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="request">The update project request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated project.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectDto>> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var project = await projectService.UpdateProjectAsync(id, request, actorUserId, cancellationToken);
        return Ok(project);
    }

    /// <summary>
    /// Deletes a project.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        await projectService.DeleteProjectAsync(id, actorUserId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Lists all members of a project.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of project members.</returns>
    [HttpGet("{id}/members")]
    public async Task<ActionResult<IEnumerable<ProjectMemberDto>>> GetProjectMembers(Guid id, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var members = await projectService.ListProjectMembersAsync(id, actorUserId, cancellationToken);
        return Ok(members);
    }

    /// <summary>
    /// Adds a member to a project.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="request">The add member request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpPost("{id}/members")]
    public async Task<IActionResult> AddProjectMember(Guid id, [FromBody] AddProjectMemberRequest request, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        await projectService.AddMemberAsync(id, request, actorUserId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Removes a member from a project.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="userId">The user ID to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}/members/{userId}")]
    public async Task<IActionResult> RemoveProjectMember(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        await projectService.RemoveMemberAsync(id, userId, actorUserId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Updates the role of a member in a project.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="request">The role update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpPut("{id}/members/{userId}/role")]
    public async Task<IActionResult> UpdateProjectMemberRole(Guid id, Guid userId, [FromBody] UserRoleUpdateRequest request, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        await projectService.UpdateMemberRoleAsync(id, userId, request.Role, actorUserId, cancellationToken);
        return NoContent();
    }

    private Guid GetActorUserId()
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(nameIdentifier, out var actorUserId))
        {
            throw new UnauthorizedException("User token is missing a valid identifier claim.");
        }

        return actorUserId;
    }
}