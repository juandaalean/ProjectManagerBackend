using Application.DTOs.Comments;
using Application.Exceptions;
using Application.Services.CommentServices;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enum;
using Moq;

namespace ProjectManager.Tests.Services;

public class CommentServiceTests
{
    private readonly Mock<ICommentRepository> _commentRepository = new();
    private readonly Mock<IProjectRepository> _projectRepository = new();
    private readonly Mock<ITaskItemRepository> _taskItemRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IUserProjectRepository> _userProjectRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private CommentService CreateService()
    {
        return new CommentService(
            _commentRepository.Object,
            _projectRepository.Object,
            _taskItemRepository.Object,
            _userRepository.Object,
            _userProjectRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task ListCommentsByTaskAsync_ShouldReturnComments_WhenActorIsMember()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _taskItemRepository.Setup(x => x.GetById(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(new TaskItem { TaskId = taskId, ProjectId = projectId, AssignedUserId = actorUserId, Title = "T", State = TaskState.Active, Priority = TaskPriority.Low });
        _userProjectRepository.Setup(x => x.GetMembership(actorUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new UserProject { UserId = actorUserId, ProjectId = projectId, RoleInProject = UserRol.User });
        _commentRepository.Setup(x => x.ListByTaskId(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(new[]
        {
            new Comment { CommentId = Guid.NewGuid(), TaskId = taskId, UserId = actorUserId, Content = "Hi", CreateAt = DateTime.UtcNow }
        });

        var service = CreateService();

        var result = await service.ListCommentsByTaskAsync(projectId, taskId, actorUserId, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task ListCommentsByTaskAsync_ShouldThrowForbidden_WhenActorIsNotMember()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _taskItemRepository.Setup(x => x.GetById(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(new TaskItem { TaskId = taskId, ProjectId = projectId, AssignedUserId = Guid.NewGuid(), Title = "T", State = TaskState.Active, Priority = TaskPriority.Low });
        _userProjectRepository.Setup(x => x.GetMembership(actorUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync((UserProject?)null);

        var service = CreateService();

        var act = () => service.ListCommentsByTaskAsync(projectId, taskId, actorUserId, CancellationToken.None);

        await Assert.ThrowsAsync<ForbiddenException>(act);
    }

    [Fact]
    public async Task CreateCommentAsync_ShouldCreateComment_WhenUserExists()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = actorUserId, Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _taskItemRepository.Setup(x => x.GetById(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(new TaskItem { TaskId = taskId, ProjectId = projectId, AssignedUserId = actorUserId, Title = "T", State = TaskState.Active, Priority = TaskPriority.Low });
        _userRepository.Setup(x => x.GetById(actorUserId, It.IsAny<CancellationToken>())).ReturnsAsync(new User { UserId = actorUserId, Name = "John" });

        var service = CreateService();

        var result = await service.CreateCommentAsync(projectId, taskId, actorUserId, new CreateCommentRequest("hello"), CancellationToken.None);

        Assert.Equal("hello", result.Content);
        Assert.Equal("John", result.UserName);
        _commentRepository.Verify(x => x.Add(It.IsAny<Comment>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateCommentAsync_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = actorUserId, Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _taskItemRepository.Setup(x => x.GetById(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(new TaskItem { TaskId = taskId, ProjectId = projectId, AssignedUserId = actorUserId, Title = "T", State = TaskState.Active, Priority = TaskPriority.Low });
        _userRepository.Setup(x => x.GetById(actorUserId, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var service = CreateService();

        var act = () => service.CreateCommentAsync(projectId, taskId, actorUserId, new CreateCommentRequest("hello"), CancellationToken.None);

        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task UpdateCommentAsync_ShouldUpdateComment_WhenActorIsAuthor()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var commentId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _taskItemRepository.Setup(x => x.GetById(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(new TaskItem { TaskId = taskId, ProjectId = projectId, AssignedUserId = actorUserId, Title = "T", State = TaskState.Active, Priority = TaskPriority.Low });

        var comment = new Comment { CommentId = commentId, TaskId = taskId, UserId = actorUserId, Content = "old", CreateAt = DateTime.UtcNow };
        _commentRepository.Setup(x => x.GetById(commentId, It.IsAny<CancellationToken>())).ReturnsAsync(comment);

        var service = CreateService();

        var result = await service.UpdateCommentAsync(projectId, taskId, commentId, actorUserId, new UpdateCommentRequest("new"), CancellationToken.None);

        Assert.Equal("new", result.Content);
        _commentRepository.Verify(x => x.Update(comment), Times.Once);
    }

    [Fact]
    public async Task UpdateCommentAsync_ShouldThrowForbidden_WhenActorIsNotAuthor()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var commentId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _taskItemRepository.Setup(x => x.GetById(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(new TaskItem { TaskId = taskId, ProjectId = projectId, AssignedUserId = actorUserId, Title = "T", State = TaskState.Active, Priority = TaskPriority.Low });
        _commentRepository.Setup(x => x.GetById(commentId, It.IsAny<CancellationToken>())).ReturnsAsync(new Comment { CommentId = commentId, TaskId = taskId, UserId = Guid.NewGuid(), Content = "old", CreateAt = DateTime.UtcNow });

        var service = CreateService();

        var act = () => service.UpdateCommentAsync(projectId, taskId, commentId, actorUserId, new UpdateCommentRequest("new"), CancellationToken.None);

        await Assert.ThrowsAsync<ForbiddenException>(act);
    }

    [Fact]
    public async Task DeleteCommentAsync_ShouldDelete_WhenActorIsAuthorAndAdmin()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var commentId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _taskItemRepository.Setup(x => x.GetById(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(new TaskItem { TaskId = taskId, ProjectId = projectId, AssignedUserId = actorUserId, Title = "T", State = TaskState.Active, Priority = TaskPriority.Low });

        var comment = new Comment { CommentId = commentId, TaskId = taskId, UserId = actorUserId, Content = "old", CreateAt = DateTime.UtcNow };
        _commentRepository.Setup(x => x.GetById(commentId, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        _userProjectRepository.Setup(x => x.GetMembership(actorUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new UserProject { UserId = actorUserId, ProjectId = projectId, RoleInProject = UserRol.Admin });

        var service = CreateService();

        await service.DeleteCommentAsync(projectId, taskId, commentId, actorUserId, CancellationToken.None);

        _commentRepository.Verify(x => x.Delete(comment), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCommentAsync_ShouldThrowForbidden_WhenActorIsNotAdmin()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var commentId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = Guid.NewGuid(), Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _taskItemRepository.Setup(x => x.GetById(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(new TaskItem { TaskId = taskId, ProjectId = projectId, AssignedUserId = actorUserId, Title = "T", State = TaskState.Active, Priority = TaskPriority.Low });
        _commentRepository.Setup(x => x.GetById(commentId, It.IsAny<CancellationToken>())).ReturnsAsync(new Comment { CommentId = commentId, TaskId = taskId, UserId = actorUserId, Content = "old", CreateAt = DateTime.UtcNow });
        _userProjectRepository.Setup(x => x.GetMembership(actorUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new UserProject { UserId = actorUserId, ProjectId = projectId, RoleInProject = UserRol.User });

        var service = CreateService();

        var act = () => service.DeleteCommentAsync(projectId, taskId, commentId, actorUserId, CancellationToken.None);

        await Assert.ThrowsAsync<ForbiddenException>(act);
    }
}
