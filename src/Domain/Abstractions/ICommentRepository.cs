using Domain.Entities;

namespace Domain.Abstractions;

public interface ICommentRepository
{
    /// <summary>
    /// Retrieves a comment by its identifier, including user information.
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The comment if found; otherwise, null.</returns>
    Task<Comment?> GetById(Guid commentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new comment to the repository.
    /// </summary>
    /// <param name="comment">The comment to add.</param>
    void Add(Comment comment);

    /// <summary>
    /// Marks a comment as updated in the repository.
    /// </summary>
    /// <param name="comment">The comment to update.</param>
    void Update(Comment comment);

    /// <summary>
    /// Removes a comment from the repository.
    /// </summary>
    /// <param name="comment">The comment to remove.</param>
    void Delete(Comment comment);

    /// <summary>
    /// Retrieves a list of comments associated with a specific task, including user information, ordered by creation date descending.
    /// </summary>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing an enumerable of comments.</returns>
    Task<IEnumerable<Comment>> ListByTaskId(Guid taskId, CancellationToken cancellationToken = default);
}