using Application.DTOs.Projects;
using FluentValidation;

namespace Application.Validators.Projects;

/// <summary>
/// Validator for project update requests.
/// </summary>
public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProjectRequestValidator"/> class.
    /// </summary>
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .When(x => x.Name is not null)
            .WithMessage("Project name is required.");

        RuleFor(x => x.Name)
            .MaximumLength(255)
            .When(x => x.Name is not null)
            .WithMessage("Project name must not exceed 255 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null)
            .WithMessage("Project description must not exceed 500 characters.");

        RuleFor(x => x)
            .Must(x => x.StartDate <= x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Start date must be before or equal to end date.");
    }
}
