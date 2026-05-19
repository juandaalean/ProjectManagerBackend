using System.Collections.Generic;

namespace Application.DTOs.TaskItem;

public record ProjectTaskItemsDto(
    Guid ProjectId,
    IEnumerable<TaskItemDto> TaskItems
);