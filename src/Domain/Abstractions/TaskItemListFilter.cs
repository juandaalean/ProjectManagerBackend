using Domain.Enum;

namespace Domain.Abstractions;

/// <summary>
/// Contract filter for listing task items.
/// </summary>
/// <param name="SearchTerm">Optional text filter by task title.</param>
/// <param name="TaskState">Optional filter by task state.</param>
/// <param name="TaskPriority">Optional filter by task priority.</param>
/// <param name="AssignedUserId">Optional filter by assigned user ID.</param>
public sealed record TaskItemListFilter(
    string? SearchTerm = null,
    TaskState? TaskState = null,
    TaskPriority? TaskPriority = null,
    Guid? AssignedUserId = null
);