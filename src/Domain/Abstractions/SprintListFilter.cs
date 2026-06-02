namespace Domain.Abstractions;
using Domain.Enum;

/// <summary>
/// Contract filter for listing sprints.
/// </summary>
/// <param name="SearchTerm">Optional text filter by sprint name.</param>
/// <param name="StartDateFrom">Optional inclusive start date lower bound.</param>
/// <param name="StartDateTo">Optional inclusive start date upper bound.</param>
/// <param name="State">Optional sprint state filter.</param>
public sealed record SprintListFilter(
    string? SearchTerm = null,
    DateTime? StartDateFrom = null,
    DateTime? StartDateTo = null,
    SprintState? State = null
);
