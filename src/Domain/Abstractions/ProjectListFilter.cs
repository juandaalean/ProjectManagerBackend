namespace Domain.Abstractions;
using Domain.Enum;

/// <summary>
/// Contract filter for listing projects.
/// </summary>
/// <param name="SearchTerm">Optional text filter by project name.</param>
/// <param name="StartDateFrom">Optional inclusive start date lower bound.</param>
/// <param name="StartDateTo">Optional inclusive start date upper bound.</param>
/// <param name="State">Optional project state filter.</param>
public sealed record ProjectListFilter(
    string? SearchTerm = null,
    DateTime? StartDateFrom = null,
    DateTime? StartDateTo = null,
    ProjectState? State = null
);