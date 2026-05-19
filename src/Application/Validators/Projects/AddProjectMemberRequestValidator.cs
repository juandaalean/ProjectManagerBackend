using Application.DTOs.Projects;
using FluentValidation;

namespace Application.Validators.Projects;

public class AddProjectMemberRequestValidator : AbstractValidator<AddProjectMemberRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddProjectMemberRequestValidator"/> class.
    /// </summary>
    public AddProjectMemberRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");
    }
}
