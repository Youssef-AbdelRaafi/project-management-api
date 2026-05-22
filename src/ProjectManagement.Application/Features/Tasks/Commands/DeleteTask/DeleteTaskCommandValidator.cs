using FluentValidation;

namespace ProjectManagement.Application.Features.Tasks.Commands.DeleteTask;

public sealed class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}
