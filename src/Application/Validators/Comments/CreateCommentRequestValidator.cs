using Application.DTOs.Comments;
using FluentValidation;

namespace Application.Validators.Comments;

/// <summary>
/// Validator for comment creation requests.
/// </summary>
public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCommentRequestValidator"/> class.
    /// </summary>
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .Must(content => !string.IsNullOrWhiteSpace(content)).WithMessage("Content must not be empty or whitespace-only.")
            .MaximumLength(500).WithMessage("Content must not exceed 500 characters.");
    }
}
