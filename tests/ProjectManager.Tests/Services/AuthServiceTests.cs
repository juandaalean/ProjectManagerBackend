using Application.DTOs.Users;
using Application.Exceptions;
using Application.Security;
using Application.Services;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enum;
using Moq;

namespace ProjectManager.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ITokenService> _tokenService = new();

    private AuthService CreateService()
    {
        return new AuthService(
            _userRepository.Object,
            _passwordHasher.Object,
            _unitOfWork.Object,
            _tokenService.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenEmailDoesNotExist()
    {
        var request = new RegisterRequest(" John ", " TEST@MAIL.COM ", "secret");
        _userRepository.Setup(x => x.EmailExists("test@mail.com", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _passwordHasher.Setup(x => x.HashPassword(It.IsAny<User>(), "secret")).Returns("hashed");
        _tokenService.Setup(x => x.GenerateAccessToken(It.IsAny<User>())).Returns(new AccessTokenResult("token", DateTime.UtcNow.AddHours(1), "Bearer"));

        var service = CreateService();

        var result = await service.RegisterAsync(request, CancellationToken.None);

        Assert.Equal("token", result.AccessToken);
        Assert.Equal("John", result.User.Name);
        Assert.Equal("test@mail.com", result.User.Email);
        _userRepository.Verify(x => x.Add(It.Is<User>(u => u.PasswordHash == "hashed")), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowValidationException_WhenEmailAlreadyExists()
    {
        var request = new RegisterRequest("John", "test@mail.com", "secret");
        _userRepository.Setup(x => x.EmailExists("test@mail.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var service = CreateService();

        var act = () => service.RegisterAsync(request, CancellationToken.None);

        await Assert.ThrowsAsync<ValidationException>(act);
        _userRepository.Verify(x => x.Add(It.IsAny<User>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        var service = CreateService();

        var act = () => service.RegisterAsync(null!, CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        var request = new LoginRequest("USER@MAIL.COM", "secret");
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Name = "John",
            Email = "user@mail.com",
            PasswordHash = "hash",
            Rol = UserRol.User
        };

        _userRepository.Setup(x => x.GetByEmail("user@mail.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.VerifyHashedPassword(user, "hash", "secret")).Returns(true);
        _tokenService.Setup(x => x.GenerateAccessToken(user)).Returns(new AccessTokenResult("token", DateTime.UtcNow.AddHours(1), "Bearer"));

        var service = CreateService();

        var result = await service.LoginAsync(request, CancellationToken.None);

        Assert.Equal("token", result.AccessToken);
        Assert.Equal(user.UserId, result.User.UserId);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenUserIsNotFound()
    {
        var request = new LoginRequest("unknown@mail.com", "secret");
        _userRepository.Setup(x => x.GetByEmail("unknown@mail.com", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var service = CreateService();

        var act = () => service.LoginAsync(request, CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenPasswordIsInvalid()
    {
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Name = "John",
            Email = "user@mail.com",
            PasswordHash = "hash",
            Rol = UserRol.User
        };

        _userRepository.Setup(x => x.GetByEmail("user@mail.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.VerifyHashedPassword(user, "hash", "bad")).Returns(false);

        var service = CreateService();

        var act = () => service.LoginAsync(new LoginRequest("user@mail.com", "bad"), CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(act);
    }
}
