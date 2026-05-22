using FluentValidation;
using ProjectManagement.Domain.Common;

namespace ProjectManagement.Application.Features.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(DomainConstants.Project.NameMaxLength);

        RuleFor(command => command.Description)
            .MaximumLength(DomainConstants.Project.DescriptionMaxLength);
    }
}
