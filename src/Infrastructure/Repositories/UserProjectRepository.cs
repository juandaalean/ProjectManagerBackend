using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing UserProject relationships.
/// </summary>
public class UserProjectRepository(ProjectManagerContext context) : IUserProjectRepository
{
    public void AddMember(UserProject userProject)
    {
        context.UserProjects.Add(userProject);
    }

    public void RemoveMember(UserProject userProject)
    {
        context.UserProjects.Remove(userProject);
    }

    public async Task<UserProject?> GetMembership(Guid userId, Guid projectId, CancellationToken cancellationToken = default)
    {
        return await context.UserProjects
            .FirstOrDefaultAsync(up => up.UserId == userId && up.ProjectId == projectId, cancellationToken);
    }

    public async Task<IEnumerable<UserProject>> ListMembers(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await context.UserProjects
            .Where(up => up.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }
}