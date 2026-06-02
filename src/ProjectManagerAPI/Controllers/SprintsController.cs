using Application.DTOs.Sprints;
using Application.Exceptions;
using Application.Services.SprintServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProjectManagerAPI.Controllers;

/// <summary>
/// Controller for managing project sprints.
/// </summary>
[ApiController]
[Route("api/projects/{projectId:guid}/sprints")]
[Authorize]
public class SprintsController(ISprintService sprintService) : ControllerBase
{
    /// <summary>
    /// Gets all sprints for a project accessible to the current user.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="query">Optional query filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of sprints for the project.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SprintDto>>> GetSprints(Guid projectId, [FromQuery] ListSprintsQuery? query, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var sprints = await sprintService.ListSprintsInProjectAsync(projectId, actorUserId, query, cancellationToken);
        return Ok(sprints);
    }

    /// <summary>
    /// Gets the sprint board for a project: all sprints grouped with their tasks, plus the backlog (tasks without a sprint).
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sprint board view organized by sprints.</returns>
    [HttpGet("board")]
    public async Task<ActionResult<IEnumerable<SprintWithTasksDto>>> GetSprintBoard(Guid projectId, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var board = await sprintService.GetSprintBoardAsync(projectId, actorUserId, cancellationToken);
        return Ok(board);
    }

    /// <summary>
    /// Gets a sprint by ID inside a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sprintId">The sprint ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sprint.</returns>
    [HttpGet("{sprintId:guid}")]
    public async Task<ActionResult<SprintDto>> GetSprintById(Guid projectId, Guid sprintId, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var sprint = await sprintService.GetSprintAsync(projectId, sprintId, actorUserId, cancellationToken);
        return Ok(sprint);
    }

    /// <summary>
    /// Gets a sprint by ID together with its tasks.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sprintId">The sprint ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sprint and its tasks.</returns>
    [HttpGet("{sprintId:guid}/tasks")]
    public async Task<ActionResult<SprintWithTasksDto>> GetSprintWithTasks(Guid projectId, Guid sprintId, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var sprint = await sprintService.GetSprintWithTasksAsync(projectId, sprintId, actorUserId, cancellationToken);
        return Ok(sprint);
    }

    /// <summary>
    /// Creates a sprint inside a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="request">The create sprint request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sprint.</returns>
    [HttpPost]
    public async Task<ActionResult<SprintDto>> CreateSprint(Guid projectId, [FromBody] CreateSprintRequest request, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var sprint = await sprintService.CreateSprintAsync(projectId, request, actorUserId, cancellationToken);
        return CreatedAtAction(nameof(GetSprintById), new { projectId, sprintId = sprint.SprintId }, sprint);
    }

    /// <summary>
    /// Updates a sprint inside a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sprintId">The sprint ID.</param>
    /// <param name="request">The update sprint request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated sprint.</returns>
    [HttpPut("{sprintId:guid}")]
    public async Task<ActionResult<SprintDto>> UpdateSprint(Guid projectId, Guid sprintId, [FromBody] UpdateSprintRequest request, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var sprint = await sprintService.UpdateSprintAsync(projectId, sprintId, request, actorUserId, cancellationToken);
        return Ok(sprint);
    }

    /// <summary>
    /// Deletes a sprint inside a project. Tasks belonging to the sprint are moved to the backlog (their SprintId is cleared).
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sprintId">The sprint ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{sprintId:guid}")]
    public async Task<IActionResult> DeleteSprint(Guid projectId, Guid sprintId, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        await sprintService.DeleteSprintAsync(projectId, sprintId, actorUserId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Assigns an existing task to a sprint. The task must belong to the same project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sprintId">The sprint ID.</param>
    /// <param name="taskItemId">The task ID to assign.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpPut("{sprintId:guid}/tasks/{taskItemId:guid}")]
    public async Task<IActionResult> AssignTaskToSprint(Guid projectId, Guid sprintId, Guid taskItemId, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        await sprintService.AssignTaskToSprintAsync(projectId, sprintId, taskItemId, actorUserId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Removes a task from a sprint, moving it to the project backlog.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sprintId">The sprint ID.</param>
    /// <param name="taskItemId">The task ID to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{sprintId:guid}/tasks/{taskItemId:guid}")]
    public async Task<IActionResult> RemoveTaskFromSprint(Guid projectId, Guid sprintId, Guid taskItemId, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        await sprintService.RemoveTaskFromSprintAsync(projectId, sprintId, taskItemId, actorUserId, cancellationToken);
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
