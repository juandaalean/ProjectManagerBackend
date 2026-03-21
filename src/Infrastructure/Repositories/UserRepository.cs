using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing User entities.
/// </summary>
public class UserRepository(ProjectManagerContext context) : IUserRepository
{
    public void Add(User user)
    {
        context.Users.Add(user);
    }

    public async Task<User?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Users.FindAsync([id], cancellationToken);
    }

    public async Task<User?> GetByEmail(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExists(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> Exists(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Users.AnyAsync(u => u.UserId == id, cancellationToken);
    }
}