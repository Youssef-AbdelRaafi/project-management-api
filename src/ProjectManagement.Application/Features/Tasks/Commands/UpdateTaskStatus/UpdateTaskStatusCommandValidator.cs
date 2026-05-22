using FluentValidation;

namespace ProjectManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

public sealed class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
{
    public UpdateTaskStatusCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();

        RuleFor(command => command.Status)
            .IsInEnum();
    }
}
