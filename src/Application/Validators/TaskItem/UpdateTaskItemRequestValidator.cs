using Application.DTOs.TaskItem;
using FluentValidation;

namespace Application.Validators.TaskItem;

/// <summary>
/// Validator for task update requests.
/// </summary>
public class UpdateTaskItemRequestValidator : AbstractValidator<UpdateTaskItemRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTaskItemRequestValidator"/> class.
    /// </summary>
    public UpdateTaskItemRequestValidator()
    {
        RuleFor(x => x.AssignedUserId)
            .NotEmpty().WithMessage("Assigned user ID is invalid.")
            .When(x => x.AssignedUserId.HasValue);

        RuleFor(x => x.Title)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .When(x => x.Title is not null)
            .WithMessage("Task title is required.");

        RuleFor(x => x.Title)
            .MaximumLength(255)
            .When(x => x.Title is not null)
            .WithMessage("Task title must not exceed 255 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null)
            .WithMessage("Task description must not exceed 500 characters.");
    }
}
