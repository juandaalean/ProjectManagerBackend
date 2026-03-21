using System.Net.Mail;
using Application.DTOs.Users;
using Application.Exceptions;
using Application.Security;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enum;

namespace Application.Services;

/// <summary>
/// Service for user registration and login.
/// </summary>
public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    ITokenService tokenService) : IAuthService
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    /// <inheritdoc/>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var name = request.Name?.Trim();
        var email = NormalizeEmail(request.Email);
        var password = request.Password;

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Name is required.");
        }

        if (!IsValidEmail(email))
        {
            throw new ValidationException("A valid email is required.");
        }

        ValidatePassword(password);

        var emailExists = await userRepository.EmailExists(email, cancellationToken);
        if (emailExists)
        {
            throw new ValidationException("Email is already registered.");
        }

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Name = name,
            Email = email,
            Rol = UserRol.User
        };

        user.PasswordHash = passwordHasher.HashPassword(user, password);

        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return BuildAuthResponse(user);
    }

    /// <inheritdoc/>
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = NormalizeEmail(request.Email);
        var password = request.Password;

        if (!IsValidEmail(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        var user = await userRepository.GetByEmail(email, cancellationToken);
        if (user is null)
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        var isPasswordValid = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        return BuildAuthResponse(user);
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var token = tokenService.GenerateAccessToken(user);

        return new AuthResponse(
            token.AccessToken,
            token.TokenType,
            token.ExpiresAtUtc,
            new AuthUserDto(user.UserId, user.Name, user.Email, user.Rol.ToString()));
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ValidationException("Password is required.");
        }

        if (password.Length < 8)
        {
            throw new ValidationException("Password must have at least 8 characters.");
        }
    }

    private static string NormalizeEmail(string email)
    {
        return email?.Trim().ToLowerInvariant() ?? string.Empty;
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}