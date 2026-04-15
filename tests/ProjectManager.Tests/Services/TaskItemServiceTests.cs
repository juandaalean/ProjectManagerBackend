using Application.DTOs.TaskItem;
using Application.Exceptions;
using Application.Services.TaskItemServices;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enum;
using Moq;

namespace ProjectManager.Tests.Services;

public class TaskItemServiceTests
{
    private readonly Mock<ITaskItemRepository> _taskItemRepository = new();
    private readonly Mock<IProjectRepository> _projectRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IUserProjectRepository> _userProjectRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private TaskItemService CreateService()
    {
        return new TaskItemService(
            _taskItemRepository.Object,
            _projectRepository.Object,
            _userRepository.Object,
            _userProjectRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task CreateTaskItemAsync_ShouldCreateTask_WhenActorIsOwner()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var assignedUserId = Guid.NewGuid();
        var project = new Project { ProjectId = projectId, OwnerId = actorUserId, Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow };

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(project);
        _userRepository.Setup(x => x.GetById(actorUserId, It.IsAny<CancellationToken>())).ReturnsAsync(new User { UserId = actorUserId, Name = "Actor" });
        _userRepository.Setup(x => x.GetById(assignedUserId, It.IsAny<CancellationToken>())).ReturnsAsync(new User { UserId = assignedUserId, Name = "Assigned" });
        _userProjectRepository.Setup(x => x.GetMembership(assignedUserId, projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProject { UserId = assignedUserId, ProjectId = projectId, RoleInProject = UserRol.User });

        var service = CreateService();
        var request = new CreateTaskItemRequest(assignedUserId, "Task", "Desc", TaskPriority.High, TaskState.Active);

        var result = await service.CreateTaskItemAsync(projectId, request, actorUserId, CancellationToken.None);

        Assert.Equal("Task", result.Title);
        _taskItemRepository.Verify(x => x.Add(It.IsAny<TaskItem>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTaskItemAsync_ShouldThrowForbidden_WhenActorIsNonPrivilegedMember()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var assignedUserId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _userRepository.Setup(x => x.GetById(actorUserId, It.IsAny<CancellationToken>())).ReturnsAsync(new User { UserId = actorUserId, Name = "Actor" });
        _userProjectRepository.Setup(x => x.GetMembership(actorUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new UserProject { UserId = actorUserId, ProjectId = projectId, RoleInProject = UserRol.User });

        var service = CreateService();
        var request = new CreateTaskItemRequest(assignedUserId, "Task", "Desc", TaskPriority.High, TaskState.Active);

        var act = () => service.CreateTaskItemAsync(projectId, request, actorUserId, CancellationToken.None);

        await Assert.ThrowsAsync<ForbiddenException>(act);
    }

    [Fact]
    public async Task GetTaskItemItemAsync_ShouldReturnTask_WhenActorIsAssignedUser()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _taskItemRepository.Setup(x => x.GetById(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(new TaskItem { TaskId = taskId, ProjectId = projectId, AssignedUserId = actorUserId, Title = "Task", State = TaskState.Active, Priority = TaskPriority.Low });

        var service = CreateService();

        var result = await service.GetTaskItemItemAsync(projectId, taskId, actorUserId, CancellationToken.None);

        Assert.Equal(taskId, result.TaskId);
    }

    [Fact]
    public async Task GetTaskItemItemAsync_ShouldThrowForbidden_WhenActorHasNoAccess()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _taskItemRepository.Setup(x => x.GetById(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(new TaskItem { TaskId = taskId, ProjectId = projectId, AssignedUserId = Guid.NewGuid(), Title = "Task", State = TaskState.Active, Priority = TaskPriority.Low });
        _userProjectRepository.Setup(x => x.GetMembership(actorUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync((UserProject?)null);

        var service = CreateService();

        var act = () => service.GetTaskItemItemAsync(projectId, taskId, actorUserId, CancellationToken.None);

        await Assert.ThrowsAsync<ForbiddenException>(act);
    }

    [Fact]
    public async Task ListTaskItemsInProjectAsync_ShouldReturnTasks_WhenActorIsMember()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var tasks = new List<TaskItem>
        {
            new() { TaskId = Guid.NewGuid(), ProjectId = projectId, AssignedUserId = actorUserId, Title = "Task", State = TaskState.Active, Priority = TaskPriority.Medium }
        };

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _userProjectRepository.Setup(x => x.GetMembership(actorUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new UserProject { UserId = actorUserId, ProjectId = projectId, RoleInProject = UserRol.User });
        _taskItemRepository.Setup(x => x.ListByProject(projectId, It.IsAny<TaskItemListFilter?>(), It.IsAny<CancellationToken>())).ReturnsAsync(tasks);

        var service = CreateService();

        var result = await service.ListTaskItemsInProjectAsync(projectId, actorUserId, new ListTaskItemsQuery("Task"), CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task ListTaskItemsInProjectAsync_ShouldThrowForbidden_WhenActorIsNotMember()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _userProjectRepository.Setup(x => x.GetMembership(actorUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync((UserProject?)null);

        var service = CreateService();

        var act = () => service.ListTaskItemsInProjectAsync(projectId, actorUserId, null, CancellationToken.None);

        await Assert.ThrowsAsync<ForbiddenException>(act);
    }

    [Fact]
    public async Task UpdateTaskItemAsync_ShouldUpdate_WhenActorIsPrivileged()
    {
        var actorUserId = Guid.NewGuid();
        var assignedUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = new TaskItem { TaskId = taskId, ProjectId = projectId, AssignedUserId = assignedUserId, Title = "Old", Description = "Old", State = TaskState.Active, Priority = TaskPriority.Low };

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _taskItemRepository.Setup(x => x.GetById(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        _userProjectRepository.Setup(x => x.GetMembership(actorUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new UserProject { UserId = actorUserId, ProjectId = projectId, RoleInProject = UserRol.Admin });
        _userRepository.Setup(x => x.GetById(assignedUserId, It.IsAny<CancellationToken>())).ReturnsAsync(new User { UserId = assignedUserId, Name = "Assigned" });
        _userProjectRepository.Setup(x => x.GetMembership(assignedUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new UserProject { UserId = assignedUserId, ProjectId = projectId, RoleInProject = UserRol.User });

        var service = CreateService();

        var result = await service.UpdateTaskItemAsync(projectId, taskId, actorUserId, new UpdateTaskItemRequest(assignedUserId, "New", "New Desc", TaskState.Finished, TaskPriority.Critical), CancellationToken.None);

        Assert.Equal("New", result.Title);
        Assert.Equal(TaskState.Finished, result.TaskState);
        _taskItemRepository.Verify(x => x.Update(task), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskItemAsync_ShouldThrowForbidden_WhenAssignedUserChangesRestrictedFields()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _taskItemRepository.Setup(x => x.GetById(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(new TaskItem { TaskId = taskId, ProjectId = projectId, AssignedUserId = actorUserId, Title = "Task", State = TaskState.Active, Priority = TaskPriority.Low });
        _userProjectRepository.Setup(x => x.GetMembership(actorUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new UserProject { UserId = actorUserId, ProjectId = projectId, RoleInProject = UserRol.User });

        var service = CreateService();

        var act = () => service.UpdateTaskItemAsync(projectId, taskId, actorUserId, new UpdateTaskItemRequest(null, "New title", null, null, null), CancellationToken.None);

        await Assert.ThrowsAsync<ForbiddenException>(act);
    }

    [Fact]
    public async Task DeleteTaskItemAsync_ShouldDelete_WhenActorIsPrivileged()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = new TaskItem { TaskId = taskId, ProjectId = projectId, AssignedUserId = Guid.NewGuid(), Title = "Task", State = TaskState.Active, Priority = TaskPriority.Low };

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _taskItemRepository.Setup(x => x.GetById(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        _userProjectRepository.Setup(x => x.GetMembership(actorUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new UserProject { UserId = actorUserId, ProjectId = projectId, RoleInProject = UserRol.Coordinator });

        var service = CreateService();

        await service.DeleteTaskItemAsync(projectId, taskId, actorUserId, CancellationToken.None);

        _taskItemRepository.Verify(x => x.Delete(task), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskItemAsync_ShouldThrowForbidden_WhenActorIsNotPrivileged()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _taskItemRepository.Setup(x => x.GetById(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(new TaskItem { TaskId = taskId, ProjectId = projectId, AssignedUserId = actorUserId, Title = "Task", State = TaskState.Active, Priority = TaskPriority.Low });
        _userProjectRepository.Setup(x => x.GetMembership(actorUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new UserProject { UserId = actorUserId, ProjectId = projectId, RoleInProject = UserRol.User });

        var service = CreateService();

        var act = () => service.DeleteTaskItemAsync(projectId, taskId, actorUserId, CancellationToken.None);

        await Assert.ThrowsAsync<ForbiddenException>(act);
    }
}
