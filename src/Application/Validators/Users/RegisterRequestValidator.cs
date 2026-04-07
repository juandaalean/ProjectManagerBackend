using Application.DTOs.Users;
using FluentValidation;

namespace Application.Validators.Users;
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterRequestValidator"/> class.
    /// </summary>
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Must(password => !string.IsNullOrWhiteSpace(password)).WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must have at least 8 characters.");
    }
}
