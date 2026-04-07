using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TaskItemRepository(ProjectManagerContext context) : ITaskItemRepository
{
    public void Add(TaskItem taskItem) => context.TaskItems.Add(taskItem);

    public void Delete(TaskItem taskItem) => context.TaskItems.Remove(taskItem);

    public async Task<TaskItem?> GetById(Guid taskItemId, CancellationToken cancellationToken = default)
    {
        return await context.TaskItems
            .FindAsync([taskItemId], cancellationToken);
    }

    public async Task<IEnumerable<TaskItem>> ListByProject(
        Guid projectId,
        TaskItemListFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.TaskItems.Where(t => t.ProjectId == projectId);

        if (filter is not null)
        {
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(t => EF.Functions.ILike(t.Title, $"%{filter.SearchTerm}%"));
            }

            if (filter.TaskState.HasValue)
            {
                query = query.Where(t => t.State == filter.TaskState.Value);
            }

            if (filter.TaskPriority.HasValue)
            {
                query = query.Where(t => t.Priority == filter.TaskPriority.Value);
            }

            if (filter.AssignedUserId.HasValue)
            {
                query = query.Where(t => t.AssignedUserId == filter.AssignedUserId.Value);
            }
        }

        return await query.ToListAsync(cancellationToken);
    }

    public void Update(TaskItem taskItem) => context.TaskItems.Update(taskItem);
}