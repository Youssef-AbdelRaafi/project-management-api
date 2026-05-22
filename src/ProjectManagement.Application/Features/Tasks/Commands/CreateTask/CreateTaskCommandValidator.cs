using FluentValidation;
using ProjectManagement.Domain.Common;

namespace ProjectManagement.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// Validates task creation requests.
/// </summary>
public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTaskCommandValidator" /> class.
    /// </summary>
    public CreateTaskCommandValidator()
    {
        RuleFor(command => command.ProjectId)
            .NotEmpty();

        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(DomainConstants.TaskItem.TitleMaxLength);

        RuleFor(command => command.Description)
            .MaximumLength(DomainConstants.TaskItem.DescriptionMaxLength);

        RuleFor(command => command.DueDate)
            .NotEmpty();

        RuleFor(command => command.Priority)
            .IsInEnum();
    }
}
