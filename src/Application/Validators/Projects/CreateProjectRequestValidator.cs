using Application.DTOs.Projects;
using FluentValidation;

namespace Application.Validators.Projects;

public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProjectRequestValidator"/> class.
    /// </summary>
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Project name is required.")
            .MaximumLength(255).WithMessage("Project name must not exceed 255 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Project description must not exceed 500 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x)
            .Must(x => x.StartDate <= x.EndDate)
            .WithMessage("Start date must be before or equal to end date.");
    }
}
