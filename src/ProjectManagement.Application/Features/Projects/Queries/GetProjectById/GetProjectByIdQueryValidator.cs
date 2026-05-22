using FluentValidation;

namespace ProjectManagement.Application.Features.Projects.Queries.GetProjectById;

public sealed class GetProjectByIdQueryValidator : AbstractValidator<GetProjectByIdQuery>
{
    public GetProjectByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty();
    }
}
