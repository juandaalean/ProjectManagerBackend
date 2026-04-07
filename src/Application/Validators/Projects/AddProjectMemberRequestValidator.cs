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
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
