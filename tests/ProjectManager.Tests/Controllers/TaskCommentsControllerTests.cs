using Application.DTOs.Comments;
using Application.Exceptions;
using Application.Services.CommentServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectManagerAPI.Controllers;

namespace ProjectManager.Tests.Controllers;

public class TaskCommentsControllerTests
{
    private readonly Mock<ICommentService> _commentService = new();

    [Fact]
    public async Task GetComments_ShouldReturnOkWithComments()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _commentService.Setup(x => x.ListCommentsByTaskAsync(projectId, taskId, actorUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new CommentDto(Guid.NewGuid(), taskId, actorUserId, "John", "Hello", DateTime.UtcNow) });

        var controller = new TaskCommentsController(_commentService.Object);
        ControllerTestHelper.SetUser(controller, actorUserId);

        var result = await controller.GetComments(projectId, taskId, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsAssignableFrom<IEnumerable<CommentDto>>(ok.Value);
        Assert.Single(payload);
    }

    [Fact]
    public async Task CreateComment_ShouldReturnCreatedAtAction()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var dto = new CommentDto(Guid.NewGuid(), taskId, actorUserId, "John", "Hello", DateTime.UtcNow);

        _commentService.Setup(x => x.CreateCommentAsync(projectId, taskId, actorUserId, It.IsAny<CreateCommentRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var controller = new TaskCommentsController(_commentService.Object);
        ControllerTestHelper.SetUser(controller, actorUserId);

        var result = await controller.CreateComment(projectId, taskId, new CreateCommentRequest("Hello"), CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(TaskCommentsController.GetComments), created.ActionName);
    }

    [Fact]
    public async Task UpdateComment_ShouldReturnOkWithUpdatedComment()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var commentId = Guid.NewGuid();
        var dto = new CommentDto(commentId, taskId, actorUserId, "John", "Updated", DateTime.UtcNow);

        _commentService.Setup(x => x.UpdateCommentAsync(projectId, taskId, commentId, actorUserId, It.IsAny<UpdateCommentRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var controller = new TaskCommentsController(_commentService.Object);
        ControllerTestHelper.SetUser(controller, actorUserId);

        var result = await controller.UpdateComment(projectId, taskId, commentId, new UpdateCommentRequest("Updated"), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<CommentDto>(ok.Value);
        Assert.Equal("Updated", payload.Content);
    }

    [Fact]
    public async Task DeleteComment_ShouldReturnNoContent()
    {
        var actorUserId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var commentId = Guid.NewGuid();

        var controller = new TaskCommentsController(_commentService.Object);
        ControllerTestHelper.SetUser(controller, actorUserId);

        var result = await controller.DeleteComment(projectId, taskId, commentId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        _commentService.Verify(x => x.DeleteCommentAsync(projectId, taskId, commentId, actorUserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetComments_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        var controller = new TaskCommentsController(_commentService.Object);
        ControllerTestHelper.SetUser(controller, null);

        var act = () => controller.GetComments(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }

    [Fact]
    public async Task CreateComment_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        var controller = new TaskCommentsController(_commentService.Object);
        ControllerTestHelper.SetUser(controller, null);

        var act = () => controller.CreateComment(Guid.NewGuid(), Guid.NewGuid(), new CreateCommentRequest("Hello"), CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }

    [Fact]
    public async Task UpdateComment_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        var controller = new TaskCommentsController(_commentService.Object);
        ControllerTestHelper.SetUser(controller, null);

        var act = () => controller.UpdateComment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new UpdateCommentRequest("Updated"), CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }

    [Fact]
    public async Task DeleteComment_ShouldThrowUnauthorized_WhenClaimIsMissing()
    {
        var controller = new TaskCommentsController(_commentService.Object);
        ControllerTestHelper.SetUser(controller, null);

        var act = () => controller.DeleteComment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }
}
