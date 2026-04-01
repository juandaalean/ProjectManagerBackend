using Application.DTOs.TaskItem;
using Application.Exceptions;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enum;

namespace Application.Services.TaskItemServices;

public class TaskItemService(
    ITaskItemRepository taskItemRepository,
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    IUserProjectRepository userProjectRepository,
    IUnitOfWork unitOfWork
) : ITaskItemService
{
    public async Task<TaskItemDto> CreateTaskItemAsync(Guid projectId, CreateTaskItemRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (projectId == Guid.Empty)
        {
            throw new ValidationException("Project ID is required.");
        }

        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Task title is required.");
        }

        if (request.AssignedUserId == Guid.Empty)
        {
            throw new ValidationException("Assigned user ID is required.");
        }

        var project = await projectRepository.GetById(projectId, cancellationToken);
        if (project is null)
        {
            throw new NotFoundException("Project not found.");
        }

        var user = await userRepository.GetById(actorUserId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var actorMembership = await userProjectRepository.GetMembership(actorUserId, projectId, cancellationToken);
        var actorIsOwner = project.OwnerId == actorUserId;

        if (!actorIsOwner && actorMembership is null)
        {
            throw new ForbiddenException("Access denied.");
        }

        if (!actorIsOwner && actorMembership is not null && !IsPrivilegedRole(actorMembership.RoleInProject))
        {
            throw new ForbiddenException("Only admins or coordinators can create tasks.");
        }

        await EnsureUserCanBeAssignedAsync(project, request.AssignedUserId, cancellationToken);

        var taskItem = new TaskItem
        {
            TaskId = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            State = request.TaskState,
            Priority = request.TaskPriority,
            ProjectId = projectId,
            AssignedUserId = request.AssignedUserId
        };

        taskItemRepository.Add(taskItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(taskItem);

    }

    public async Task<TaskItemDto> GetTaskItemItemAsync(Guid projectId, Guid taskItemId, Guid actorUserId, CancellationToken cancellationToken = default)
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
        if (!actorIsOwner && taskItem.AssignedUserId != actorUserId)
        {
            var membership = await userProjectRepository.GetMembership(actorUserId, taskItem.ProjectId, cancellationToken);
            if (membership is null)
            {
                throw new ForbiddenException("Access denied.");
            }
        }

        return MapToDto(taskItem);
    }

    public async Task<IEnumerable<TaskItemDto>> ListTaskItemsInProjectAsync(Guid projectId, Guid actorUserId, ListTaskItemsQuery? query = null, CancellationToken cancellationToken = default)
    {
        if (projectId == Guid.Empty)
        {
            throw new ValidationException("Project ID is required.");
        }

        var project = await projectRepository.GetById(projectId, cancellationToken);
        if (project is null)
        {
            throw new NotFoundException("Project not found.");
        }

        if (project.OwnerId != actorUserId)
        {
            var membership = await userProjectRepository.GetMembership(actorUserId, projectId, cancellationToken);
            if (membership is null)
            {
                throw new ForbiddenException("Access denied.");
            }
        }

        TaskItemListFilter? filter = null;
        if (query is not null)
        {
            filter = new TaskItemListFilter(
                query.SearchTerm,
                query.TaskState,
                query.TaskPriority,
                query.AssignedUser);
        }

        var tasks = await taskItemRepository.ListByProject(projectId, filter, cancellationToken);

        return tasks.Select(MapToDto);
    }

    public async Task<TaskItemDto> UpdateTaskItemAsync(Guid projectId, Guid taskItemId, Guid actorUserId, UpdateTaskItemRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

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

        var actorMembership = await userProjectRepository.GetMembership(actorUserId, projectId, cancellationToken);
        var actorIsOwner = project.OwnerId == actorUserId;
        var actorIsPrivileged = actorIsOwner || (actorMembership is not null && IsPrivilegedRole(actorMembership.RoleInProject));
        var actorIsAssignedUser = taskItem.AssignedUserId == actorUserId;

        if (!actorIsPrivileged)
        {
            if (!actorIsAssignedUser)
            {
                throw new ForbiddenException("Access denied.");
            }

            var tryingToChangeRestrictedFields =
                request.AssignedUserId.HasValue ||
                request.Title is not null ||
                request.Description is not null;

            if (tryingToChangeRestrictedFields)
            {
                throw new ForbiddenException("Users can only update task state or priority.");
            }
        }

        if (request.AssignedUserId.HasValue)
        {
            if (request.AssignedUserId.Value == Guid.Empty)
            {
                throw new ValidationException("Assigned user ID is invalid.");
            }

            await EnsureUserCanBeAssignedAsync(project, request.AssignedUserId.Value, cancellationToken);
            taskItem.AssignedUserId = request.AssignedUserId.Value;
        }

        if (request.Title is not null)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ValidationException("Task title is required.");
            }

            taskItem.Title = request.Title;
        }

        if (request.Description is not null)
        {
            taskItem.Description = request.Description;
        }

        if (request.TaskState.HasValue)
        {
            taskItem.State = request.TaskState.Value;
        }

        if (request.TaskPriority.HasValue)
        {
            taskItem.Priority = request.TaskPriority.Value;
        }

        taskItemRepository.Update(taskItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(taskItem);
    }

    public async Task DeleteTaskItemAsync(Guid projectId, Guid taskItemId, Guid actorUserId, CancellationToken cancellationToken = default)
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

        var actorMembership = await userProjectRepository.GetMembership(actorUserId, projectId, cancellationToken);
        var actorIsOwner = project.OwnerId == actorUserId;
        var actorIsPrivileged = actorIsOwner || (actorMembership is not null && IsPrivilegedRole(actorMembership.RoleInProject));

        if (!actorIsPrivileged)
        {
            throw new ForbiddenException("Only admins or coordinators can delete tasks.");
        }

        taskItemRepository.Delete(taskItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUserCanBeAssignedAsync(Project project, Guid userId, CancellationToken cancellationToken)
    {
        var assignedUser = await userRepository.GetById(userId, cancellationToken);
        if (assignedUser is null)
        {
            throw new NotFoundException("Assigned user not found.");
        }

        if (project.OwnerId == userId)
        {
            return;
        }

        var membership = await userProjectRepository.GetMembership(userId, project.ProjectId, cancellationToken);
        if (membership is null)
        {
            throw new ValidationException("Assigned user must belong to the project.");
        }
    }

    private static bool IsPrivilegedRole(UserRol role) =>
        role is UserRol.Admin or UserRol.Coordinator;

    private static TaskItemDto MapToDto(TaskItem taskItem) =>
    new(
        taskItem.TaskId,
        taskItem.Title,
        taskItem.Description,
        taskItem.State,
        taskItem.Priority,
        taskItem.ProjectId,
        taskItem.AssignedUserId
    );

}
