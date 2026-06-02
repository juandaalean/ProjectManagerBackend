using Application.DTOs.TaskItem;
using Application.Exceptions;
using Application.Services.TaskItemServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectManagerAPI.Controllers;

namespace ProjectManager.Tests.Controllers;

public class TaskItemsControllerTests
{
    private readonly Mock<ITaskItemService> _taskItemService = new();

    [Fact]
    public async Task GetTasksByProjects_ShouldReturnOkWithGroupedTasks()
    {
        var actorUserId = Guid.NewGuid();
        var projectIdOne = Guid.NewGuid();
        var projectIdTwo = Guid.NewGuid();

        _taskItemService
            .Setup(x => x.ListTaskItemsByProjectsAsync(It.IsAny<IEnumerable<Guid>>(), actorUserId, It.IsAny<ListTaskItemsQuery?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new ProjectTaskItemsDto(projectIdOne, new[] { new TaskItemDto(Guid.NewGuid(), "Task 1", null, Domain.Enum.TaskState.Active, Domain.Enum.TaskPriority.Low, projectIdOne, actorUserId) }),
                new ProjectTaskItemsDto(projectIdTwo, new[] { new TaskItemDto(Guid.NewGuid(), "Task 2", null, Domain.Enum.TaskState.Active, Domain.Enum.TaskPriority.Medium, projectIdTwo, actorUserId) })
            });

        var controller = new TaskItemsController(_taskItemService.Object);
        ControllerTestHelper.SetUser(controller, actorUserId);

        var result = await controller.GetTasksByProjects(new[] { projectIdOne, projectIdTwo }, null, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsAssignableFrom<IEnumerable<ProjectTaskItemsDto>>(ok.Value);
        Assert.Equal(2, payload.Count());
    }

    [Fact]
    public async Task GetTasksByProjects_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        var controller = new TaskItemsController(_taskItemService.Object);
        ControllerTestHelper.SetUser(controller, null);

        var act = () => controller.GetTasksByProjects(new[] { Guid.NewGuid() }, null, CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }
}