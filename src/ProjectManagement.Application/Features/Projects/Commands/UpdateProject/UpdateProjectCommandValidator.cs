using FluentValidation;
using ProjectManagement.Domain.Common;

namespace ProjectManagement.Application.Features.Projects.Commands.UpdateProject;

/// <summary>
/// Validates project update requests.
/// </summary>
public sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProjectCommandValidator" /> class.
    /// </summary>
    public UpdateProjectCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(DomainConstants.Project.NameMaxLength);

        RuleFor(command => command.Description)
            .MaximumLength(DomainConstants.Project.DescriptionMaxLength);
    }
}
