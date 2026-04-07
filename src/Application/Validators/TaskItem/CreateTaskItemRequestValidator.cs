using Application.DTOs.TaskItem;
using FluentValidation;

namespace Application.Validators.TaskItem;

/// <summary>
/// Validator for task creation requests.
/// </summary>
public class CreateTaskItemRequestValidator : AbstractValidator<CreateTaskItemRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTaskItemRequestValidator"/> class.
    /// </summary>
    public CreateTaskItemRequestValidator()
    {
        RuleFor(x => x.AssignedUserId)
            .NotEmpty().WithMessage("Assigned user ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .Must(title => !string.IsNullOrWhiteSpace(title)).WithMessage("Task title is required.")
            .MaximumLength(255).WithMessage("Task title must not exceed 255 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Task description must not exceed 500 characters.")
            .When(x => x.Description is not null);
    }
}
