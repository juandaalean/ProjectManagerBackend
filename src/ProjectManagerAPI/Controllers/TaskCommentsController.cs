using Application.DTOs.Comments;
using Application.Exceptions;
using Application.Services.CommentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProjectManagerAPI.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/tasks/{taskItemId:guid}/comments")]
[Authorize]
public class TaskCommentsController(ICommentService commentService) : ControllerBase
{
    /// <summary>
    /// Gets all comments for a specific task within a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="taskItemId">The task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of comments for the task.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(Guid projectId, Guid taskItemId, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var comments = await commentService.ListCommentsByTaskAsync(projectId, taskItemId, actorUserId, cancellationToken);
        return Ok(comments);
    }

    /// <summary>
    /// Creates a new comment for a specific task within a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="taskItemId">The task ID.</param>
    /// <param name="request">The create comment request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created comment.</returns>
    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment(Guid projectId, Guid taskItemId, [FromBody] CreateCommentRequest request, CancellationToken cancellationToken)
    {
        var actorUserId = GetActorUserId();
        var comment = await commentService.CreateCommentAsync(projectId, taskItemId, actorUserId, request, cancellationToken);
        return CreatedAtAction(nameof(GetComments), new { projectId, taskItemId }, comment);
    }

    /// <summary>
    /// Gets the actor user ID from the current user's claims.
    /// </summary>
    /// <returns>The actor user ID.</returns>
    /// <exception cref="UnauthorizedException">Thrown when the user token is missing a valid identifier claim.</exception>
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