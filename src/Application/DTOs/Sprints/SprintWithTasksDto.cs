using Application.DTOs.TaskItem;

namespace Application.DTOs.Sprints;

/// <summary>
/// Response DTO grouping the tasks that belong to a sprint.
/// </summary>
/// <param name="Sprint">The sprint metadata. Will be null when representing the project backlog (tasks with no sprint).</param>
/// <param name="Tasks">The collection of tasks that belong to the sprint (or to the backlog).</param>
public record SprintWithTasksDto(
    SprintDto? Sprint,
    IEnumerable<TaskItemDto> Tasks
);
