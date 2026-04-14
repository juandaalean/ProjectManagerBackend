using Application.DTOs.Comments;
using FluentValidation;

namespace Application.Validators.Comments;

public class UpdateCommentRequestValidator : AbstractValidator<UpdateCommentRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCommentRequestValidator"/> class.
    /// </summary>
    public UpdateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .Must(content => !string.IsNullOrWhiteSpace(content)).WithMessage("Content must not be empty or whitespace-only.")
            .MaximumLength(500).WithMessage("Content must not exceed 500 characters.");
    }
}
