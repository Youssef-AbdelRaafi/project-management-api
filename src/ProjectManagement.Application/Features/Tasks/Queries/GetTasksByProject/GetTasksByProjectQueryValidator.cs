using FluentValidation;

namespace ProjectManagement.Application.Features.Tasks.Queries.GetTasksByProject;

/// <summary>
/// Validates task list queries.
/// </summary>
public sealed class GetTasksByProjectQueryValidator : AbstractValidator<GetTasksByProjectQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTasksByProjectQueryValidator" /> class.
    /// </summary>
    public GetTasksByProjectQueryValidator()
    {
        RuleFor(query => query.ProjectId)
            .NotEmpty();

        RuleFor(query => query.Status)
            .IsInEnum()
            .When(query => query.Status.HasValue);
    }
}
