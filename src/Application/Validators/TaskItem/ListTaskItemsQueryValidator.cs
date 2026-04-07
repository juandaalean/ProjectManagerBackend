using Application.DTOs.TaskItem;
using FluentValidation;

namespace Application.Validators.TaskItem;

/// <summary>
/// Validator for list-tasks query filters.
/// </summary>
public class ListTaskItemsQueryValidator : AbstractValidator<ListTaskItemsQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListTaskItemsQueryValidator"/> class.
    /// </summary>
    public ListTaskItemsQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm))
            .WithMessage("Search term must not exceed 255 characters.");

        RuleFor(x => x.AssignedUser)
            .NotEmpty().WithMessage("Assigned user filter is invalid.")
            .When(x => x.AssignedUser.HasValue);
    }
}
