using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enum;

namespace Application.DTOs.TaskItem
{
    public record CreateTaskItemRequest(
        Guid AssignedUserId,
        string Title,
        string? Description,
        TaskPriority TaskPriority,
        TaskState TaskState = TaskState.Active
    );
}