using Application.DTOs.Projects;
using FluentValidation;

namespace Application.Validators.Projects;

public class ListProjectsQueryValidator : AbstractValidator<ListProjectsQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListProjectsQueryValidator"/> class.
    /// </summary>
    public ListProjectsQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm))
            .WithMessage("Search term must not exceed 255 characters.");

        RuleFor(x => x)
            .Must(x => x.StartDateFrom <= x.StartDateTo)
            .When(x => x.StartDateFrom.HasValue && x.StartDateTo.HasValue)
            .WithMessage("StartDateFrom must be before or equal to StartDateTo.");
    }
}
