using Application.DTOs.Users;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectManagerAPI.Controllers;

namespace ProjectManager.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authService = new();

    [Fact]
    public async Task Register_ShouldReturnOkWithAuthResponse()
    {
        var response = new AuthResponse("token", "Bearer", DateTime.UtcNow.AddHours(1), new AuthUserDto(Guid.NewGuid(), "John", "john@mail.com", "User"));
        _authService.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var controller = new AuthController(_authService.Object);

        var result = await controller.Register(new RegisterRequest("John", "john@mail.com", "secret"), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AuthResponse>(ok.Value);
        Assert.Equal("token", payload.AccessToken);
    }

    [Fact]
    public async Task Login_ShouldReturnOkWithAuthResponse()
    {
        var response = new AuthResponse("token", "Bearer", DateTime.UtcNow.AddHours(1), new AuthUserDto(Guid.NewGuid(), "John", "john@mail.com", "User"));
        _authService.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var controller = new AuthController(_authService.Object);

        var result = await controller.Login(new LoginRequest("john@mail.com", "secret"), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AuthResponse>(ok.Value);
        Assert.Equal("token", payload.AccessToken);
    }

    [Fact]
    public async Task Login_ShouldCallServiceWithSameRequest()
    {
        var request = new LoginRequest("john@mail.com", "secret");
        _authService.Setup(x => x.LoginAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(new AuthResponse("t", "Bearer", DateTime.UtcNow, new AuthUserDto(Guid.NewGuid(), "John", "john@mail.com", "User")));

        var controller = new AuthController(_authService.Object);

        await controller.Login(request, CancellationToken.None);

        _authService.Verify(x => x.LoginAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_ShouldCallServiceWithSameRequest()
    {
        var request = new RegisterRequest("John", "john@mail.com", "secret");
        _authService.Setup(x => x.RegisterAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(new AuthResponse("t", "Bearer", DateTime.UtcNow, new AuthUserDto(Guid.NewGuid(), "John", "john@mail.com", "User")));

        var controller = new AuthController(_authService.Object);

        await controller.Register(request, CancellationToken.None);

        _authService.Verify(x => x.RegisterAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}
