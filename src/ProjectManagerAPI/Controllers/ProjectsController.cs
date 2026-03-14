using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManagerAPI.Controllers;

/// <summary>
/// Controller for managing projects.
/// </summary>
[ApiController]
[Route("api/projects")]
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
        var actorUserId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
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
        var actorUserId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
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
        var actorUserId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
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
        var actorUserId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        await projectService.DeleteProjectAsync(id, actorUserId, cancellationToken);
        return NoContent();
    }
}