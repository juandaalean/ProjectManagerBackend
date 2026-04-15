using Application.DTOs.Projects;
using Application.Exceptions;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectManagerAPI.Controllers;

namespace ProjectManager.Tests.Controllers;

public class ProjectsControllerTests
{
    private readonly Mock<IProjectService> _projectService = new();

    [Fact]
    public async Task GetProjects_ShouldReturnOkWithProjects_WhenUserClaimIsValid()
    {
        var actorUserId = Guid.NewGuid();
        _projectService.Setup(x => x.ListProjectsForUserAsync(actorUserId, It.IsAny<ListProjectsQuery?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new ProjectDto(Guid.NewGuid(), "Project", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), actorUserId) });

        var controller = new ProjectsController(_projectService.Object);
        ControllerTestHelper.SetUser(controller, actorUserId);

        var result = await controller.GetProjects(null, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(ok.Value);
        Assert.Single(payload);
    }

    [Fact]
    public async Task CreateProject_ShouldReturnCreatedAtAction()
    {
        var actorUserId = Guid.NewGuid();
        var project = new ProjectDto(Guid.NewGuid(), "Project", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), actorUserId);
        _projectService.Setup(x => x.CreateProjectAsync(It.IsAny<CreateProjectRequest>(), actorUserId, It.IsAny<CancellationToken>())).ReturnsAsync(project);

        var controller = new ProjectsController(_projectService.Object);
        ControllerTestHelper.SetUser(controller, actorUserId);

        var result = await controller.CreateProject(new CreateProjectRequest("Project", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ProjectsController.GetProjects), created.ActionName);
    }

    [Fact]
    public async Task UpdateProject_ShouldReturnOkWithUpdatedProject()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var project = new ProjectDto(projectId, "Updated", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), actorUserId);
        _projectService.Setup(x => x.UpdateProjectAsync(projectId, It.IsAny<UpdateProjectRequest>(), actorUserId, It.IsAny<CancellationToken>())).ReturnsAsync(project);

        var controller = new ProjectsController(_projectService.Object);
        ControllerTestHelper.SetUser(controller, actorUserId);

        var result = await controller.UpdateProject(projectId, new UpdateProjectRequest("Updated", null, null, null), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<ProjectDto>(ok.Value);
        Assert.Equal(projectId, payload.ProjectId);
    }

    [Fact]
    public async Task DeleteProject_ShouldReturnNoContent()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        var controller = new ProjectsController(_projectService.Object);
        ControllerTestHelper.SetUser(controller, actorUserId);

        var result = await controller.DeleteProject(projectId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        _projectService.Verify(x => x.DeleteProjectAsync(projectId, actorUserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProjects_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        var controller = new ProjectsController(_projectService.Object);
        ControllerTestHelper.SetUser(controller, null);

        var act = () => controller.GetProjects(null, CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }

    [Fact]
    public async Task CreateProject_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        var controller = new ProjectsController(_projectService.Object);
        ControllerTestHelper.SetUser(controller, null);

        var act = () => controller.CreateProject(new CreateProjectRequest("Project", null, DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }

    [Fact]
    public async Task UpdateProject_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        var controller = new ProjectsController(_projectService.Object);
        ControllerTestHelper.SetUser(controller, null);

        var act = () => controller.UpdateProject(Guid.NewGuid(), new UpdateProjectRequest("New", null, null, null), CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }

    [Fact]
    public async Task DeleteProject_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        var controller = new ProjectsController(_projectService.Object);
        ControllerTestHelper.SetUser(controller, null);

        var act = () => controller.DeleteProject(Guid.NewGuid(), CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }
}
