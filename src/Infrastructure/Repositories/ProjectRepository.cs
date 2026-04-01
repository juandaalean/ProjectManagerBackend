using Domain.Abstractions;
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

    public async Task<IEnumerable<Project>> ListByUser(
        Guid userId,
        ProjectListFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Projects
            .Where(p => p.OwnerId == userId || p.UserProjects.Any(up => up.UserId == userId));

        if (filter is not null)
        {
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(p => EF.Functions.ILike(p.Name, $"%{filter.SearchTerm}%"));
            }

            if (filter.StartDateFrom.HasValue)
            {
                query = query.Where(p => p.StartDate >= filter.StartDateFrom.Value);
            }

            if (filter.StartDateTo.HasValue)
            {
                query = query.Where(p => p.StartDate <= filter.StartDateTo.Value);
            }
        }

        return await query.ToListAsync(cancellationToken);
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