using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing User entities.
/// </summary>
public class UserRepository(ProjectManagerContext context) : IUserRepository
{
    /// <inheritdoc />
    public async Task<User?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Users.FindAsync([id], cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmail(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> Exists(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Users.AnyAsync(u => u.UserId == id, cancellationToken);
    }
}