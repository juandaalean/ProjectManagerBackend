using Domain.Enum;

namespace Application.DTOs.TaskItem
{
    public record UpdateTaskItemRequest(
        Guid? AssignedUserId,
        string? Title,
        string? Description,
        TaskState? TaskState,
        TaskPriority? TaskPriority,
        DateTime? CreatedAt = null,
        DateTime? StartAt = null,
        DateTime? CompletedAt = null,
        Guid? SprintId = null,
        bool ClearSprint = false,
        bool ClearCompletedAt = false
    );
}