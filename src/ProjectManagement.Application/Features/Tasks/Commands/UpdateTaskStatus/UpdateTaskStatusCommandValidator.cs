using FluentValidation;

namespace ProjectManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

/// <summary>
/// Validates task status update requests.
/// </summary>
public sealed class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTaskStatusCommandValidator" /> class.
    /// </summary>
    public UpdateTaskStatusCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();

        RuleFor(command => command.Status)
            .IsInEnum();
    }
}
