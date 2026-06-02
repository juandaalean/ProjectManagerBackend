using Application.DTOs.Sprints;
using FluentValidation;

namespace Application.Validators.Sprints;

public class UpdateSprintRequestValidator : AbstractValidator<UpdateSprintRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSprintRequestValidator"/> class.
    /// </summary>
    public UpdateSprintRequestValidator()
    {
        RuleFor(x => x.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .When(x => x.Name is not null)
            .WithMessage("Sprint name is required.");

        RuleFor(x => x.Name)
            .MaximumLength(255)
            .When(x => x.Name is not null)
            .WithMessage("Sprint name must not exceed 255 characters.");

        RuleFor(x => x.Goal)
            .MaximumLength(500)
            .When(x => x.Goal is not null)
            .WithMessage("Sprint goal must not exceed 500 characters.");

        RuleFor(x => x)
            .Must(x => x.StartDate <= x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Start date must be before or equal to end date.");

        RuleFor(x => x.State)
            .IsInEnum()
            .When(x => x.State.HasValue)
            .WithMessage("Sprint state is invalid.");
    }
}
