using Application.DTOs.TaskItem;
using Application.Exceptions;
using Application.Services.TaskItemServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProjectManagerAPI.Controllers;

/// <summary>
/// Controller for managing project tasks.
/// </summary>
[ApiController]
[Route("api/projects/{projectId:guid}/tasks")]
[Authorize]
public class TaskItemController(ITaskItemService taskItemService) : ControllerBase
{
    /// <summary>
    /// Gets all tasks for a project accessible to the current user.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="query">Optional query filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of tasks for the project.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasks(Guid projectId, [FromQuery] ListTaskItemsQuery? query, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var tasks = await taskItemService.ListTaskItemsInProjectAsync(projectId, actorUserId, query, cancellationToken);
        return Ok(tasks);
    }

    /// <summary>
    /// Gets a task by ID inside a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="taskItemId">The task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task.</returns>
    [HttpGet("{taskItemId:guid}")]
    public async Task<ActionResult<TaskItemDto>> GetTaskById(Guid projectId, Guid taskItemId, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var task = await taskItemService.GetTaskItemItemAsync(projectId, taskItemId, actorUserId, cancellationToken);
        return Ok(task);
    }

    /// <summary>
    /// Creates a task inside a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="request">The create task request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created task.</returns>
    [HttpPost]
    public async Task<ActionResult<TaskItemDto>> CreateTask(Guid projectId, [FromBody] CreateTaskItemRequest request, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var task = await taskItemService.CreateTaskItemAsync(projectId, request, actorUserId, cancellationToken);
        return CreatedAtAction(nameof(GetTaskById), new { projectId, taskItemId = task.TaskId }, task);
    }

    /// <summary>
    /// Updates a task inside a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="taskItemId">The task ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated task.</returns>
    [HttpPut("{taskItemId:guid}")]
    public async Task<ActionResult<TaskItemDto>> UpdateTask(Guid projectId, Guid taskItemId, [FromBody] UpdateTaskItemRequest request, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var task = await taskItemService.UpdateTaskItemAsync(projectId, taskItemId, actorUserId, request, cancellationToken);
        return Ok(task);
    }

    /// <summary>
    /// Deletes a task inside a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="taskItemId">The task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{taskItemId:guid}")]
    public async Task<IActionResult> DeleteTask(Guid projectId, Guid taskItemId, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        await taskItemService.DeleteTaskItemAsync(projectId, taskItemId, actorUserId, cancellationToken);
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