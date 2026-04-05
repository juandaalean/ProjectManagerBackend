using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CommentRepository(ProjectManagerContext context) : ICommentRepository
{
    /// <summary>
    /// Adds a new comment to the repository.
    /// </summary>
    /// <param name="comment">The comment to add.</param>
    public void Add(Comment comment) => context.Comments.Add(comment);

    /// <summary>
    /// Retrieves a list of comments associated with a specific task, including user information, ordered by creation date descending.
    /// </summary>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing an enumerable of comments.</returns>
    public async Task<IEnumerable<Comment>> ListByTaskId(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await context.Comments
            .Where(c => c.TaskId == taskId)
            .Include(c => c.User)
            .OrderByDescending(c => c.CreateAt)
            .ToListAsync(cancellationToken);
    }
}