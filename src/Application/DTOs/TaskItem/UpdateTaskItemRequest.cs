using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        DateTime? CompletedAt = null,
        Guid? SprintId = null,
        bool ClearSprint = false
    );
}