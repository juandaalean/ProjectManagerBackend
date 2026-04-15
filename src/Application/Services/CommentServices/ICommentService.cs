using Application.DTOs.Comments;

namespace Application.Services.CommentServices;

public interface ICommentService
{
    /// <summary>
    /// Lists comments for a specific task within a project, checking authorization.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="taskItemId">The task ID.</param>
    /// <param name="actorUserId">The acting user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task containing the list of comment DTOs.</returns>
    Task<IEnumerable<CommentDto>> ListCommentsByTaskAsync(Guid projectId, Guid taskItemId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new comment for a task within a project, checking authorization.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="taskItemId">The task ID.</param>
    /// <param name="actorUserId">The acting user ID.</param>
    /// <param name="request">The create comment request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task containing the created comment DTO.</returns>
    Task<CommentDto> CreateCommentAsync(Guid projectId, Guid taskItemId, Guid actorUserId, CreateCommentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing comment for a task within a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="taskItemId">The task ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="actorUserId">The acting user ID.</param>
    /// <param name="request">The update comment request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task containing the updated comment DTO.</returns>
    Task<CommentDto> UpdateCommentAsync(Guid projectId, Guid taskItemId, Guid commentId, Guid actorUserId, UpdateCommentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing comment for a task within a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="taskItemId">The task ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="actorUserId">The acting user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteCommentAsync(Guid projectId, Guid taskItemId, Guid commentId, Guid actorUserId, CancellationToken cancellationToken = default);
}