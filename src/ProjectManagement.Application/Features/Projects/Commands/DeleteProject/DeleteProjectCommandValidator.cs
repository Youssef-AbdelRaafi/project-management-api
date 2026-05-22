using FluentValidation;

namespace ProjectManagement.Application.Features.Projects.Commands.DeleteProject;

public sealed class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}
