using Domain.Enum;

namespace Application.DTOs.TaskItem;

public record TaskItemDto(
    Guid TaskId,
    string Title,
    string? Description,
    TaskState TaskState,
    TaskPriority TaskPriority,
    Guid ProjectId,
    Guid AssignedUserId,
    DateTime CreatedAt = default,
    DateTime? CompletedAt = null,
    Guid? SprintId = null
);
