using Domain.Enum;

namespace Application.DTOs.TaskItem;

public record ListTaskItemsQuery(
    string? SearchTerm = null,
    TaskState? TaskState = null,
    TaskPriority? TaskPriority = null,
    Guid? AssignedUser = null
);