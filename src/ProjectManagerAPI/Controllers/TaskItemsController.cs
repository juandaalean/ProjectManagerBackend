using Application.DTOs.TaskItem;
using Application.Exceptions;
using Application.Services.TaskItemServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProjectManagerAPI.Controllers;

/// <summary>
/// Controller for querying task collections across projects.
/// </summary>
[ApiController]
[Route("api/task-items")]
[Authorize]
public class TaskItemsController(ITaskItemService taskItemService) : ControllerBase
{
    /// <summary>
    /// Gets the tasks for multiple projects the current user can access.
    /// </summary>
    /// <param name="projectIds">The project IDs to query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks grouped by project.</returns>
    [HttpGet("by-projects")]
    public async Task<ActionResult<IEnumerable<ProjectTaskItemsDto>>> GetTasksByProjects([FromQuery] Guid[] projectIds, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var tasksByProject = await taskItemService.ListTaskItemsByProjectsAsync(projectIds, actorUserId, cancellationToken);
        return Ok(tasksByProject);
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