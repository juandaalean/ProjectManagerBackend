using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TaskItemRepository(ProjectManagerContext context) : ITaskItemRepository
    {
        public void Add(TaskItem taskItem) => context.TaskItems.Add(taskItem);
        

        public void Delete(TaskItem taskItem) => context.Remove(taskItem);

        public async Task<TaskItem?> GetById(Guid taskItemId, CancellationToken cancellationToken = default)
        {
            return await context.TaskItems
                .FindAsync([taskItemId], cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> ListByProject(Guid projectId, CancellationToken cancellationToken = default)
        {
            return await context.TaskItems
                .Where(t => t.ProjectId == projectId)
                .ToListAsync(cancellationToken);
        }

        public void Update(TaskItem taskItem) => context.TaskItems.Update(taskItem);
    }
}