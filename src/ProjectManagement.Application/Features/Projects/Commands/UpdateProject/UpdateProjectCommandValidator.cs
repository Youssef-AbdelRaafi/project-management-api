using FluentValidation;
using ProjectManagement.Domain.Common;

namespace ProjectManagement.Application.Features.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
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
