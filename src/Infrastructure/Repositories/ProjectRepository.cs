using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing Project entities.
/// </summary>
public class ProjectRepository(ProjectManagerContext context) : IProjectRepository
{
    public void Add(Project project)
    {
        context.Projects.Add(project);
    }

    public async Task<Project?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Projects.FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<Project>> ListByUser(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Projects
            .Where(p => p.OwnerId == userId)
            .ToListAsync(cancellationToken);
    }

    public void Update(Project project)
    {
        context.Projects.Update(project);
    }

    public void Delete(Project project)
    {
        context.Projects.Remove(project);
    }
}