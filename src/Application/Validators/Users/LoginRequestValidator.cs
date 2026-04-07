using Application.DTOs.Users;
using FluentValidation;

namespace Application.Validators.Users;

/// <summary>
/// Validator for user login requests.
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginRequestValidator"/> class.
    /// </summary>
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Must(password => !string.IsNullOrWhiteSpace(password)).WithMessage("Password is required.");
    }
}
