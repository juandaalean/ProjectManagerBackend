using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enum;

namespace Application.DTOs.TaskItem
{
    public record ListTaskItemsQuery(
        string? SearchTerm = null,
        TaskState? TaskState = null,
        TaskPriority? TaskPriority = null,
        Guid? AssignedUser = null
    );
}