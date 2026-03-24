using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.TaskItem;

namespace Application.Services.TaskItemServices
{
    public interface ITaskItemService
    {
        Task<TaskItemDto> CreateTaskItemAsync(Guid projectId, CreateTaskItemRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

        Task<TaskItemDto> GetTaskItemItemAsync(Guid projectId, Guid taskItemId, Guid actorUserId, CancellationToken cancellationToken = default);

        Task<IEnumerable<TaskItemDto>> ListTaskItemsInProjectAsync(Guid projectId, Guid actionUserId, ListTaskItemsQuery? query = null, CancellationToken cancellationToken = default);

        Task<TaskItemDto> UpdateTaskItemAsync(Guid projectId, Guid taskItemId, Guid actorUserId, UpdateTaskItemRequest request, CancellationToken cancellationToken = default);

        Task DeleteTaskItemAsync(Guid projectId, Guid taskItemId, Guid actorUserId, CancellationToken cancellationToken = default);
    }
}