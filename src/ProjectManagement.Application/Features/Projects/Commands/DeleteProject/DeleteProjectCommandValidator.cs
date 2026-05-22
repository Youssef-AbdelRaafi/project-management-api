using FluentValidation;

namespace ProjectManagement.Application.Features.Projects.Commands.DeleteProject;

/// <summary>
/// Validates project delete requests.
/// </summary>
public sealed class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteProjectCommandValidator" /> class.
    /// </summary>
    public DeleteProjectCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}
