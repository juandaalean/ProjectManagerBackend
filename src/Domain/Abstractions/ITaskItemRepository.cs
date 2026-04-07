using Domain.Entities;

namespace Domain.Abstractions;

public interface ITaskItemRepository
{
    void Add(TaskItem taskItem);
    Task<TaskItem?> GetById(Guid taskItemId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> ListByProject(
        Guid projectId,
        TaskItemListFilter? filter = null,
        CancellationToken cancellationToken = default);
    void Update(TaskItem taskItem);
    void Delete(TaskItem taskItem);
}