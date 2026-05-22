using FluentValidation;
using ProjectManagement.Domain.Common;

namespace ProjectManagement.Application.Features.Projects.Commands.CreateProject;

/// <summary>
/// Validates project creation requests.
/// </summary>
public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProjectCommandValidator" /> class.
    /// </summary>
    public CreateProjectCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(DomainConstants.Project.NameMaxLength);

        RuleFor(command => command.Description)
            .MaximumLength(DomainConstants.Project.DescriptionMaxLength);
    }
}
