using Domain.Entities;

namespace Domain.Abstractions;

public interface ICommentRepository
{
    /// <summary>
    /// Adds a new comment to the repository.
    /// </summary>
    /// <param name="comment">The comment to add.</param>
    void Add(Comment comment);

    /// <summary>
    /// Retrieves a list of comments associated with a specific task, including user information, ordered by creation date descending.
    /// </summary>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing an enumerable of comments.</returns>
    Task<IEnumerable<Comment>> ListByTaskId(Guid taskId, CancellationToken cancellationToken = default);
}