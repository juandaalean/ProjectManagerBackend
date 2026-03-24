using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enum;

namespace Application.DTOs.TaskItem;
    public record TaskItemDto(
        Guid TaskId,
        string Title,
        string? Description,
        TaskState TaskState,
        TaskPriority TaskPriority
    );
