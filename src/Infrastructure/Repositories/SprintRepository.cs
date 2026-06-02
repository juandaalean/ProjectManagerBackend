using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing Sprint entities.
/// </summary>
public class SprintRepository(ProjectManagerContext context) : ISprintRepository
{
    public void Add(Sprint sprint) => context.Sprints.Add(sprint);

    public void Update(Sprint sprint) => context.Sprints.Update(sprint);

    public void Delete(Sprint sprint) => context.Sprints.Remove(sprint);

    public async Task<Sprint?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Sprints.FindAsync([id], cancellationToken);
    }

    public async Task<Sprint?> GetByIdWithTasks(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Sprints
            .Include(s => s.Tasks)
            .FirstOrDefaultAsync(s => s.SprintId == id, cancellationToken);
    }

    public async Task<IEnumerable<Sprint>> ListByProject(
        Guid projectId,
        SprintListFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Sprints.Where(s => s.ProjectId == projectId);

        if (filter is not null)
        {
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(s => EF.Functions.ILike(s.Name, $"%{filter.SearchTerm}%"));
            }

            if (filter.StartDateFrom.HasValue)
            {
                query = query.Where(s => s.StartDate >= filter.StartDateFrom.Value);
            }

            if (filter.StartDateTo.HasValue)
            {
                query = query.Where(s => s.StartDate <= filter.StartDateTo.Value);
            }

            if (filter.State.HasValue)
            {
                query = query.Where(s => s.State == filter.State.Value);
            }
        }

        return await query
            .OrderBy(s => s.StartDate)
            .ThenBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sprint>> ListByProjectWithTasks(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        return await context.Sprints
            .Where(s => s.ProjectId == projectId)
            .Include(s => s.Tasks)
            .OrderBy(s => s.StartDate)
            .ThenBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }
}
