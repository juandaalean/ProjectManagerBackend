using Application.DTOs.Projects;
using Application.Exceptions;
using Application.Services;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enum;
using Moq;

namespace ProjectManager.Tests.Services;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IUserProjectRepository> _userProjectRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private ProjectService CreateService()
    {
        return new ProjectService(
            _projectRepository.Object,
            _userRepository.Object,
            _userProjectRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task CreateProjectAsync_ShouldCreateProject_WhenUserExists()
    {
        var actorUserId = Guid.NewGuid();
        var request = new CreateProjectRequest("Project A", "Desc", DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(10));

        _userRepository.Setup(x => x.GetById(actorUserId, It.IsAny<CancellationToken>())).ReturnsAsync(new User { UserId = actorUserId, Name = "Owner" });

        var service = CreateService();

        var result = await service.CreateProjectAsync(request, actorUserId, CancellationToken.None);

        Assert.Equal("Project A", result.Name);
        Assert.Equal(actorUserId, result.OwnerId);
        _projectRepository.Verify(x => x.Add(It.IsAny<Project>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProjectAsync_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        var actorUserId = Guid.NewGuid();
        _userRepository.Setup(x => x.GetById(actorUserId, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var service = CreateService();

        var act = () => service.CreateProjectAsync(new CreateProjectRequest("P", null, DateTime.UtcNow, DateTime.UtcNow), actorUserId, CancellationToken.None);

        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task GetProjectAsync_ShouldReturnProject_WhenActorIsOwner()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var project = new Project
        {
            ProjectId = projectId,
            Name = "Project",
            OwnerId = actorUserId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1)
        };

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(project);

        var service = CreateService();

        var result = await service.GetProjectAsync(projectId, actorUserId, CancellationToken.None);

        Assert.Equal(projectId, result.ProjectId);
    }

    [Fact]
    public async Task GetProjectAsync_ShouldThrowForbidden_WhenActorIsNotMember()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project
        {
            ProjectId = projectId,
            Name = "Project",
            OwnerId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow
        });
        _userProjectRepository.Setup(x => x.GetMembership(actorUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync((UserProject?)null);

        var service = CreateService();

        var act = () => service.GetProjectAsync(projectId, actorUserId, CancellationToken.None);

        await Assert.ThrowsAsync<ForbiddenException>(act);
    }

    [Fact]
    public async Task ListProjectsForUserAsync_ShouldReturnMappedProjects()
    {
        var actorUserId = Guid.NewGuid();
        var projects = new List<Project>
        {
            new()
            {
                ProjectId = Guid.NewGuid(),
                Name = "A",
                OwnerId = actorUserId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow
            }
        };

        _projectRepository.Setup(x => x.ListByUser(actorUserId, It.IsAny<ProjectListFilter?>(), It.IsAny<CancellationToken>())).ReturnsAsync(projects);

        var service = CreateService();

        var result = await service.ListProjectsForUserAsync(actorUserId, new ListProjectsQuery("abc"), CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task ListProjectsForUserAsync_ShouldPassNullFilter_WhenQueryIsNull()
    {
        var actorUserId = Guid.NewGuid();
        _projectRepository.Setup(x => x.ListByUser(actorUserId, null, It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<Project>());

        var service = CreateService();

        await service.ListProjectsForUserAsync(actorUserId, null, CancellationToken.None);

        _projectRepository.Verify(x => x.ListByUser(actorUserId, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProjectAsync_ShouldUpdateProject_WhenActorIsOwner()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var project = new Project
        {
            ProjectId = projectId,
            Name = "Old",
            OwnerId = actorUserId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow
        };

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(project);

        var service = CreateService();

        var result = await service.UpdateProjectAsync(projectId, new UpdateProjectRequest("New", "Desc", null, null), actorUserId, CancellationToken.None);

        Assert.Equal("New", result.Name);
        _projectRepository.Verify(x => x.Update(project), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProjectAsync_ShouldThrowForbidden_WhenActorIsNotOwner()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project
        {
            ProjectId = projectId,
            Name = "P",
            OwnerId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow
        });

        var service = CreateService();

        var act = () => service.UpdateProjectAsync(projectId, new UpdateProjectRequest("N", null, null, null), actorUserId, CancellationToken.None);

        await Assert.ThrowsAsync<ForbiddenException>(act);
    }

    [Fact]
    public async Task DeleteProjectAsync_ShouldDelete_WhenActorIsOwner()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var project = new Project { ProjectId = projectId, OwnerId = actorUserId, Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow };
        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(project);

        var service = CreateService();

        await service.DeleteProjectAsync(projectId, actorUserId, CancellationToken.None);

        _projectRepository.Verify(x => x.Delete(project), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProjectAsync_ShouldThrowForbidden_WhenActorIsNotOwner()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project
        {
            ProjectId = projectId,
            OwnerId = Guid.NewGuid(),
            Name = "P",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow
        });

        var service = CreateService();

        var act = () => service.DeleteProjectAsync(projectId, actorUserId, CancellationToken.None);

        await Assert.ThrowsAsync<ForbiddenException>(act);
    }

    [Fact]
    public async Task AddMemberAsync_ShouldAddMember_WhenOwnerAndUserExists()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = actorUserId, Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _userRepository.Setup(x => x.GetById(targetUserId, It.IsAny<CancellationToken>())).ReturnsAsync(new User { UserId = targetUserId, Name = "U" });
        _userProjectRepository.Setup(x => x.GetMembership(targetUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync((UserProject?)null);

        var service = CreateService();

        await service.AddMemberAsync(projectId, new AddProjectMemberRequest(targetUserId, UserRol.User), actorUserId, CancellationToken.None);

        _userProjectRepository.Verify(x => x.AddMember(It.Is<UserProject>(m => m.UserId == targetUserId && m.ProjectId == projectId)), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddMemberAsync_ShouldThrowValidation_WhenMembershipAlreadyExists()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = actorUserId, Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _userRepository.Setup(x => x.GetById(targetUserId, It.IsAny<CancellationToken>())).ReturnsAsync(new User { UserId = targetUserId, Name = "U" });
        _userProjectRepository.Setup(x => x.GetMembership(targetUserId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new UserProject { UserId = targetUserId, ProjectId = projectId });

        var service = CreateService();

        var act = () => service.AddMemberAsync(projectId, new AddProjectMemberRequest(targetUserId, UserRol.User), actorUserId, CancellationToken.None);

        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task RemoveMemberAsync_ShouldRemoveMember_WhenMembershipExists()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var membership = new UserProject { UserId = userId, ProjectId = projectId };

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = actorUserId, Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _userProjectRepository.Setup(x => x.GetMembership(userId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync(membership);

        var service = CreateService();

        await service.RemoveMemberAsync(projectId, userId, actorUserId, CancellationToken.None);

        _userProjectRepository.Verify(x => x.RemoveMember(membership), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveMemberAsync_ShouldThrowValidation_WhenTryingToRemoveOwner()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = actorUserId, Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });

        var service = CreateService();

        var act = () => service.RemoveMemberAsync(projectId, actorUserId, actorUserId, CancellationToken.None);

        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_ShouldUpdateRole_WhenMembershipExists()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var membership = new UserProject { UserId = userId, ProjectId = projectId, RoleInProject = UserRol.User };

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = actorUserId, Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _userProjectRepository.Setup(x => x.GetMembership(userId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync(membership);

        var service = CreateService();

        await service.UpdateMemberRoleAsync(projectId, userId, UserRol.Admin, actorUserId, CancellationToken.None);

        Assert.Equal(UserRol.Admin, membership.RoleInProject);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_ShouldThrowNotFound_WhenMembershipDoesNotExist()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _projectRepository.Setup(x => x.GetById(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { ProjectId = projectId, OwnerId = actorUserId, Name = "P", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });
        _userProjectRepository.Setup(x => x.GetMembership(userId, projectId, It.IsAny<CancellationToken>())).ReturnsAsync((UserProject?)null);

        var service = CreateService();

        var act = () => service.UpdateMemberRoleAsync(projectId, userId, UserRol.Admin, actorUserId, CancellationToken.None);

        await Assert.ThrowsAsync<NotFoundException>(act);
    }
}
