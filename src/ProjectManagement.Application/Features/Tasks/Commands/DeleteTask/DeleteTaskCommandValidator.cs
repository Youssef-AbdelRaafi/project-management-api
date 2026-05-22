using FluentValidation;

namespace ProjectManagement.Application.Features.Tasks.Commands.DeleteTask;

/// <summary>
/// Validates task deletion requests.
/// </summary>
public sealed class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTaskCommandValidator" /> class.
    /// </summary>
    public DeleteTaskCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}
