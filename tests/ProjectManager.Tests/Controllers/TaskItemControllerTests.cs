using Application.DTOs.TaskItem;
using Application.Exceptions;
using Application.Services.TaskItemServices;
using Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectManagerAPI.Controllers;

namespace ProjectManager.Tests.Controllers;

public class TaskItemControllerTests
{
    private readonly Mock<ITaskItemService> _taskItemService = new();

    [Fact]
    public async Task GetTasks_ShouldReturnOkWithTasks()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        _taskItemService.Setup(x => x.ListTaskItemsInProjectAsync(projectId, actorUserId, It.IsAny<ListTaskItemsQuery?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new TaskItemDto(Guid.NewGuid(), "Task", "Desc", TaskState.Active, TaskPriority.Low, projectId, actorUserId) });

        var controller = new TaskItemController(_taskItemService.Object);
        ControllerTestHelper.SetUser(controller, actorUserId);

        var result = await controller.GetTasks(projectId, null, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsAssignableFrom<IEnumerable<TaskItemDto>>(ok.Value);
        Assert.Single(payload);
    }

    [Fact]
    public async Task GetTaskById_ShouldReturnOkWithTask()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        _taskItemService.Setup(x => x.GetTaskItemItemAsync(projectId, taskId, actorUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TaskItemDto(taskId, "Task", "Desc", TaskState.Active, TaskPriority.Low, projectId, actorUserId));

        var controller = new TaskItemController(_taskItemService.Object);
        ControllerTestHelper.SetUser(controller, actorUserId);

        var result = await controller.GetTaskById(projectId, taskId, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<TaskItemDto>(ok.Value);
        Assert.Equal(taskId, payload.TaskId);
    }

    [Fact]
    public async Task CreateTask_ShouldReturnCreatedAtAction()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var dto = new TaskItemDto(Guid.NewGuid(), "Task", "Desc", TaskState.Active, TaskPriority.Medium, projectId, actorUserId);
        _taskItemService.Setup(x => x.CreateTaskItemAsync(projectId, It.IsAny<CreateTaskItemRequest>(), actorUserId, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var controller = new TaskItemController(_taskItemService.Object);
        ControllerTestHelper.SetUser(controller, actorUserId);

        var result = await controller.CreateTask(projectId, new CreateTaskItemRequest(actorUserId, "Task", "Desc", TaskPriority.Medium), CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(TaskItemController.GetTaskById), created.ActionName);
    }

    [Fact]
    public async Task UpdateTask_ShouldReturnOkWithUpdatedTask()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        _taskItemService.Setup(x => x.UpdateTaskItemAsync(projectId, taskId, actorUserId, It.IsAny<UpdateTaskItemRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TaskItemDto(taskId, "Updated", "Desc", TaskState.Finished, TaskPriority.High, projectId, actorUserId));

        var controller = new TaskItemController(_taskItemService.Object);
        ControllerTestHelper.SetUser(controller, actorUserId);

        var result = await controller.UpdateTask(projectId, taskId, new UpdateTaskItemRequest(null, null, null, TaskState.Finished, null), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<TaskItemDto>(ok.Value);
        Assert.Equal(TaskState.Finished, payload.TaskState);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNoContent()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var controller = new TaskItemController(_taskItemService.Object);
        ControllerTestHelper.SetUser(controller, actorUserId);

        var result = await controller.DeleteTask(projectId, taskId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        _taskItemService.Verify(x => x.DeleteTaskItemAsync(projectId, taskId, actorUserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTasks_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        var controller = new TaskItemController(_taskItemService.Object);
        ControllerTestHelper.SetUser(controller, null);

        var act = () => controller.GetTasks(Guid.NewGuid(), null, CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }

    [Fact]
    public async Task GetTaskById_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        var controller = new TaskItemController(_taskItemService.Object);
        ControllerTestHelper.SetUser(controller, null);

        var act = () => controller.GetTaskById(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }

    [Fact]
    public async Task CreateTask_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        var controller = new TaskItemController(_taskItemService.Object);
        ControllerTestHelper.SetUser(controller, null);

        var act = () => controller.CreateTask(Guid.NewGuid(), new CreateTaskItemRequest(Guid.NewGuid(), "Task", null, TaskPriority.Low), CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }

    [Fact]
    public async Task UpdateTask_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        var controller = new TaskItemController(_taskItemService.Object);
        ControllerTestHelper.SetUser(controller, null);

        var act = () => controller.UpdateTask(Guid.NewGuid(), Guid.NewGuid(), new UpdateTaskItemRequest(null, null, null, TaskState.Active, null), CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }

    [Fact]
    public async Task DeleteTask_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        var controller = new TaskItemController(_taskItemService.Object);
        ControllerTestHelper.SetUser(controller, null);

        var act = () => controller.DeleteTask(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }
}
