using Application.DTOs.Sprints;
using Application.DTOs.TaskItem;
using Application.Exceptions;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enum;

namespace Application.Services.SprintServices;

/// <summary>
/// Service for handling sprint-related operations.
/// </summary>
public class SprintService(
    ISprintRepository sprintRepository,
    IProjectRepository projectRepository,
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository,
    IUserProjectRepository userProjectRepository,
    IUnitOfWork unitOfWork) : ISprintService
{
    public async Task<SprintDto> CreateSprintAsync(Guid projectId, CreateSprintRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (projectId == Guid.Empty)
        {
            throw new ValidationException("Project ID is required.");
        }

        var project = await projectRepository.GetById(projectId, cancellationToken);
        if (project is null)
        {
            throw new NotFoundException("Project not found.");
        }

        await EnsureActorCanManageSprintsAsync(project, actorUserId, cancellationToken, "Only the project owner or a project admin/coordinator can create sprints.");

        var sprint = new Sprint
        {
            SprintId = Guid.NewGuid(),
            ProjectId = projectId,
            Name = request.Name,
            Goal = request.Goal,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            State = SprintState.Planned
        };

        sprintRepository.Add(sprint);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(sprint);
    }

    public async Task<SprintDto> GetSprintAsync(Guid projectId, Guid sprintId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var (_, sprint) = await LoadProjectAndSprintAsync(projectId, sprintId, cancellationToken);
        await EnsureActorCanReadProjectAsync(sprint.ProjectId, actorUserId, cancellationToken);

        return MapToDto(sprint);
    }

    public async Task<IEnumerable<SprintDto>> ListSprintsInProjectAsync(Guid projectId, Guid actorUserId, ListSprintsQuery? query = null, CancellationToken cancellationToken = default)
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

        await EnsureActorIsProjectMemberAsync(project, actorUserId, cancellationToken);

        SprintListFilter? filter = null;
        if (query is not null)
        {
            filter = new SprintListFilter(
                query.SearchTerm,
                query.StartDateFrom,
                query.StartDateTo,
                query.State);
        }

        var sprints = await sprintRepository.ListByProject(projectId, filter, cancellationToken);

        return sprints.Select(MapToDto);
    }

    public async Task<IEnumerable<SprintWithTasksDto>> GetSprintBoardAsync(Guid projectId, Guid actorUserId, CancellationToken cancellationToken = default)
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

        await EnsureActorIsProjectMemberAsync(project, actorUserId, cancellationToken);

        var sprints = await sprintRepository.ListByProject(projectId, filter: null, cancellationToken);
        var allTasks = await taskItemRepository.ListByProject(projectId, filter: null, cancellationToken);

        var tasksBySprint = allTasks
            .Where(t => t.SprintId.HasValue)
            .GroupBy(t => t.SprintId!.Value)
            .ToDictionary(g => g.Key, g => g.OrderBy(t => t.CreatedAt).ToList());

        var backlogTasks = allTasks
            .Where(t => t.SprintId is null)
            .OrderBy(t => t.CreatedAt)
            .Select(MapTaskToDto)
            .ToList();

        var board = new List<SprintWithTasksDto>
        {
            new(null, backlogTasks)
        };

        var orderedSprints = sprints
            .OrderBy(s => s.StartDate)
            .ThenBy(s => s.Name);

        foreach (var sprint in orderedSprints)
        {
            var sprintTasks = tasksBySprint.TryGetValue(sprint.SprintId, out var grouped)
                ? grouped.Select(MapTaskToDto)
                : Enumerable.Empty<TaskItemDto>();

            board.Add(new SprintWithTasksDto(MapToDto(sprint), sprintTasks));
        }

        return board;
    }

    public async Task<SprintWithTasksDto> GetSprintWithTasksAsync(Guid projectId, Guid sprintId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (projectId == Guid.Empty)
        {
            throw new ValidationException("Project ID is required.");
        }

        if (sprintId == Guid.Empty)
        {
            throw new ValidationException("Sprint ID is required.");
        }

        var sprint = await sprintRepository.GetByIdWithTasks(sprintId, cancellationToken);
        if (sprint is null || sprint.ProjectId != projectId)
        {
            throw new NotFoundException("Sprint not found.");
        }

        await EnsureActorCanReadProjectAsync(sprint.ProjectId, actorUserId, cancellationToken);

        var tasks = sprint.Tasks
            .OrderBy(t => t.CreatedAt)
            .Select(MapTaskToDto);

        return new SprintWithTasksDto(MapToDto(sprint), tasks);
    }

    public async Task<SprintDto> UpdateSprintAsync(Guid projectId, Guid sprintId, UpdateSprintRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var (project, sprint) = await LoadProjectAndSprintAsync(projectId, sprintId, cancellationToken);

        await EnsureActorCanManageSprintsAsync(project, actorUserId, cancellationToken, "Only the project owner or a project admin/coordinator can update sprints.");

        if (request.Name is not null)
        {
            sprint.Name = request.Name;
        }

        if (request.Goal is not null)
        {
            sprint.Goal = request.Goal;
        }

        if (request.StartDate.HasValue)
        {
            sprint.StartDate = request.StartDate.Value;
        }

        if (request.EndDate.HasValue)
        {
            sprint.EndDate = request.EndDate.Value;
        }

        if (request.State.HasValue)
        {
            sprint.State = request.State.Value;
        }

        if (sprint.StartDate > sprint.EndDate)
        {
            throw new ValidationException("Start date must be before or equal to end date.");
        }

        sprintRepository.Update(sprint);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(sprint);
    }

    public async Task DeleteSprintAsync(Guid projectId, Guid sprintId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var (project, sprint) = await LoadProjectAndSprintAsync(projectId, sprintId, cancellationToken);

        await EnsureActorCanManageSprintsAsync(project, actorUserId, cancellationToken, "Only the project owner or a project admin/coordinator can delete sprints.");

        sprintRepository.Delete(sprint);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignTaskToSprintAsync(Guid projectId, Guid sprintId, Guid taskItemId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var (project, sprint) = await LoadProjectAndSprintAsync(projectId, sprintId, cancellationToken);

        await EnsureActorCanManageSprintsAsync(project, actorUserId, cancellationToken, "Only the project owner or a project admin/coordinator can assign tasks to sprints.");

        if (taskItemId == Guid.Empty)
        {
            throw new ValidationException("Task ID is required.");
        }

        var taskItem = await taskItemRepository.GetById(taskItemId, cancellationToken);
        if (taskItem is null || taskItem.ProjectId != projectId)
        {
            throw new NotFoundException("Task not found.");
        }

        taskItem.SprintId = sprint.SprintId;
        taskItemRepository.Update(taskItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveTaskFromSprintAsync(Guid projectId, Guid sprintId, Guid taskItemId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var (project, sprint) = await LoadProjectAndSprintAsync(projectId, sprintId, cancellationToken);

        await EnsureActorCanManageSprintsAsync(project, actorUserId, cancellationToken, "Only the project owner or a project admin/coordinator can remove tasks from sprints.");

        if (taskItemId == Guid.Empty)
        {
            throw new ValidationException("Task ID is required.");
        }

        var taskItem = await taskItemRepository.GetById(taskItemId, cancellationToken);
        if (taskItem is null || taskItem.ProjectId != projectId)
        {
            throw new NotFoundException("Task not found.");
        }

        if (taskItem.SprintId != sprint.SprintId)
        {
            throw new ValidationException("The task does not belong to this sprint.");
        }

        taskItem.SprintId = null;
        taskItemRepository.Update(taskItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<(Project Project, Sprint Sprint)> LoadProjectAndSprintAsync(Guid projectId, Guid sprintId, CancellationToken cancellationToken)
    {
        if (projectId == Guid.Empty)
        {
            throw new ValidationException("Project ID is required.");
        }

        if (sprintId == Guid.Empty)
        {
            throw new ValidationException("Sprint ID is required.");
        }

        var project = await projectRepository.GetById(projectId, cancellationToken);
        if (project is null)
        {
            throw new NotFoundException("Project not found.");
        }

        var sprint = await sprintRepository.GetById(sprintId, cancellationToken);
        if (sprint is null || sprint.ProjectId != projectId)
        {
            throw new NotFoundException("Sprint not found.");
        }

        return (project, sprint);
    }

    private async Task EnsureActorIsProjectMemberAsync(Project project, Guid actorUserId, CancellationToken cancellationToken)
    {
        if (project.OwnerId == actorUserId)
        {
            return;
        }

        var membership = await userProjectRepository.GetMembership(actorUserId, project.ProjectId, cancellationToken);
        if (membership is null)
        {
            throw new ForbiddenException("Access denied.");
        }
    }

    private async Task EnsureActorCanReadProjectAsync(Guid projectId, Guid actorUserId, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetById(projectId, cancellationToken);
        if (project is null)
        {
            throw new NotFoundException("Project not found.");
        }

        await EnsureActorIsProjectMemberAsync(project, actorUserId, cancellationToken);
    }

    private async Task EnsureActorCanManageSprintsAsync(Project project, Guid actorUserId, CancellationToken cancellationToken, string forbiddenMessage)
    {
        var actor = await userRepository.GetById(actorUserId, cancellationToken);
        if (actor is null)
        {
            throw new NotFoundException("Actor user not found.");
        }

        if (project.OwnerId == actorUserId)
        {
            return;
        }

        var membership = await userProjectRepository.GetMembership(actorUserId, project.ProjectId, cancellationToken);
        if (membership is null || !IsPrivilegedRole(membership.RoleInProject))
        {
            throw new ForbiddenException(forbiddenMessage);
        }
    }

    private static bool IsPrivilegedRole(UserRol role) =>
        role is UserRol.Admin or UserRol.Coordinator;

    private static SprintDto MapToDto(Sprint sprint) =>
        new(
            sprint.SprintId,
            sprint.ProjectId,
            sprint.Name,
            sprint.Goal,
            sprint.StartDate,
            sprint.EndDate,
            sprint.State
        );

    private static TaskItemDto MapTaskToDto(TaskItem taskItem) =>
        new(
            taskItem.TaskId,
            taskItem.Title,
            taskItem.Description,
            taskItem.State,
            taskItem.Priority,
            taskItem.ProjectId,
            taskItem.AssignedUserId,
            taskItem.CreatedAt,
            taskItem.CompletedAt,
            taskItem.StartAt,
            taskItem.SprintId
        );
}
