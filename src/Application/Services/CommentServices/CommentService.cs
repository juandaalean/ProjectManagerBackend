using Application.DTOs.Comments;
using Application.Exceptions;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enum;

namespace Application.Services.CommentServices;

/// <summary>
/// Service implementation for comment operations.
/// </summary>
public class CommentService(
    ICommentRepository commentRepository,
    IProjectRepository projectRepository,
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository,
    IUserProjectRepository userProjectRepository,
    IUnitOfWork unitOfWork
) : ICommentService
{
    /// <summary>
    /// Lists comments for a specific task within a project, checking authorization.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="taskItemId">The task ID.</param>
    /// <param name="actorUserId">The acting user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task containing the list of comment DTOs.</returns>
    public async Task<IEnumerable<CommentDto>> ListCommentsByTaskAsync(Guid projectId, Guid taskItemId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (projectId == Guid.Empty)
        {
            throw new ValidationException("Project ID is required.");
        }

        if (taskItemId == Guid.Empty)
        {
            throw new ValidationException("Task ID is required.");
        }

        var project = await projectRepository.GetById(projectId, cancellationToken);
        if (project is null)
        {
            throw new NotFoundException("Project not found.");
        }

        var taskItem = await taskItemRepository.GetById(taskItemId, cancellationToken);
        if (taskItem is null || taskItem.ProjectId != projectId)
        {
            throw new NotFoundException("Task not found.");
        }

        var actorIsOwner = project.OwnerId == actorUserId;
        if (!actorIsOwner)
        {
            var membership = await userProjectRepository.GetMembership(actorUserId, projectId, cancellationToken);
            if (membership is null)
            {
                throw new ForbiddenException("Access denied.");
            }
        }

        var comments = await commentRepository.ListByTaskId(taskItemId, cancellationToken);
        return comments.Select(MapToDto);
    }

    /// <summary>
    /// Creates a new comment for a task within a project, checking authorization.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="taskItemId">The task ID.</param>
    /// <param name="actorUserId">The acting user ID.</param>
    /// <param name="request">The create comment request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task containing the created comment DTO.</returns>
    public async Task<CommentDto> CreateCommentAsync(Guid projectId, Guid taskItemId, Guid actorUserId, CreateCommentRequest request, CancellationToken cancellationToken = default)
    {
        if (projectId == Guid.Empty)
        {
            throw new ValidationException("Project ID is required.");
        }

        if (taskItemId == Guid.Empty)
        {
            throw new ValidationException("Task ID is required.");
        }

        ArgumentNullException.ThrowIfNull(request);

        var project = await projectRepository.GetById(projectId, cancellationToken);
        if (project is null)
        {
            throw new NotFoundException("Project not found.");
        }

        var taskItem = await taskItemRepository.GetById(taskItemId, cancellationToken);
        if (taskItem is null || taskItem.ProjectId != projectId)
        {
            throw new NotFoundException("Task not found.");
        }

        var actorIsOwner = project.OwnerId == actorUserId;
        if (!actorIsOwner)
        {
            var membership = await userProjectRepository.GetMembership(actorUserId, projectId, cancellationToken);
            if (membership is null)
            {
                throw new ForbiddenException("Access denied.");
            }
        }

        var user = await userRepository.GetById(actorUserId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var comment = new Comment
        {
            CommentId = Guid.NewGuid(),
            Content = request.Content,
            CreateAt = DateTime.UtcNow,
            UserId = actorUserId,
            TaskId = taskItemId
        };

        commentRepository.Add(comment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(comment, user.Name);
    }

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
    public async Task<CommentDto> UpdateCommentAsync(Guid projectId, Guid taskItemId, Guid commentId, Guid actorUserId, UpdateCommentRequest request, CancellationToken cancellationToken = default)
    {
        if (projectId == Guid.Empty)
        {
            throw new ValidationException("Project ID is required.");
        }

        if (taskItemId == Guid.Empty)
        {
            throw new ValidationException("Task ID is required.");
        }

        if (commentId == Guid.Empty)
        {
            throw new ValidationException("Comment ID is required.");
        }

        ArgumentNullException.ThrowIfNull(request);

        var project = await projectRepository.GetById(projectId, cancellationToken);
        if (project is null)
        {
            throw new NotFoundException("Project not found.");
        }

        var taskItem = await taskItemRepository.GetById(taskItemId, cancellationToken);
        if (taskItem is null || taskItem.ProjectId != projectId)
        {
            throw new NotFoundException("Task not found.");
        }

        var comment = await commentRepository.GetById(commentId, cancellationToken);
        if (comment is null || comment.TaskId != taskItemId)
        {
            throw new NotFoundException("Comment not found.");
        }

        if (comment.UserId != actorUserId)
        {
            throw new ForbiddenException("Only the comment author can edit it.");
        }

        comment.Content = request.Content;

        commentRepository.Update(comment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(comment);
    }

    /// <summary>
    /// Deletes an existing comment for a task within a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="taskItemId">The task ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="actorUserId">The acting user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DeleteCommentAsync(Guid projectId, Guid taskItemId, Guid commentId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (projectId == Guid.Empty)
        {
            throw new ValidationException("Project ID is required.");
        }

        if (taskItemId == Guid.Empty)
        {
            throw new ValidationException("Task ID is required.");
        }

        if (commentId == Guid.Empty)
        {
            throw new ValidationException("Comment ID is required.");
        }

        var project = await projectRepository.GetById(projectId, cancellationToken);
        if (project is null)
        {
            throw new NotFoundException("Project not found.");
        }

        var taskItem = await taskItemRepository.GetById(taskItemId, cancellationToken);
        if (taskItem is null || taskItem.ProjectId != projectId)
        {
            throw new NotFoundException("Task not found.");
        }

        var comment = await commentRepository.GetById(commentId, cancellationToken);
        if (comment is null || comment.TaskId != taskItemId)
        {
            throw new NotFoundException("Comment not found.");
        }

        if (comment.UserId != actorUserId)
        {
            throw new ForbiddenException("Only the comment author can delete it.");
        }

        var membership = await userProjectRepository.GetMembership(actorUserId, projectId, cancellationToken);
        if (membership is null || membership.RoleInProject != UserRol.Admin)
        {
            throw new ForbiddenException("Only project admins can delete comments.");
        }

        commentRepository.Delete(comment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Maps a Comment entity to a CommentDto.
    /// </summary>
    /// <param name="comment">The comment entity.</param>
    /// <returns>The comment DTO.</returns>
    private static CommentDto MapToDto(Comment comment)
    {
        return new CommentDto(
            comment.CommentId,
            comment.TaskId,
            comment.UserId,
            comment.User?.Name ?? string.Empty,
            comment.Content,
            comment.CreateAt
        );
    }

    /// <summary>
    /// Maps a Comment entity to a CommentDto with a specific user name.
    /// </summary>
    /// <param name="comment">The comment entity.</param>
    /// <param name="userName">The user name.</param>
    /// <returns>The comment DTO.</returns>
    private static CommentDto MapToDto(Comment comment, string userName)
    {
        return new CommentDto(
            comment.CommentId,
            comment.TaskId,
            comment.UserId,
            userName,
            comment.Content,
            comment.CreateAt
        );
    }
}