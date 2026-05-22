using FluentValidation;

namespace ProjectManagement.Application.Features.Projects.Queries.GetProjectById;

/// <summary>
/// Validates project details queries.
/// </summary>
public sealed class GetProjectByIdQueryValidator : AbstractValidator<GetProjectByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectByIdQueryValidator" /> class.
    /// </summary>
    public GetProjectByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty();
    }
}
