using Application.DTOs.Sprints;
using FluentValidation;

namespace Application.Validators.Sprints;

public class CreateSprintRequestValidator : AbstractValidator<CreateSprintRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSprintRequestValidator"/> class.
    /// </summary>
    public CreateSprintRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Sprint name is required.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Sprint name is required.")
            .MaximumLength(255).WithMessage("Sprint name must not exceed 255 characters.");

        RuleFor(x => x.Goal)
            .MaximumLength(500).WithMessage("Sprint goal must not exceed 500 characters.")
            .When(x => x.Goal is not null);

        RuleFor(x => x)
            .Must(x => x.StartDate <= x.EndDate)
            .WithMessage("Start date must be before or equal to end date.");
    }
}
