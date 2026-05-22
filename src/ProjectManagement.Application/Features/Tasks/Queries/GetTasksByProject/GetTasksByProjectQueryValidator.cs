using FluentValidation;

namespace ProjectManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public sealed class GetTasksByProjectQueryValidator : AbstractValidator<GetTasksByProjectQuery>
{
    public GetTasksByProjectQueryValidator()
    {
        RuleFor(query => query.ProjectId)
            .NotEmpty();

        RuleFor(query => query.Status)
            .IsInEnum()
            .When(query => query.Status.HasValue);
    }
}
