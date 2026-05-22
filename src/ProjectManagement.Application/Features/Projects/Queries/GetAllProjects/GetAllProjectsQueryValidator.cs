using FluentValidation;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Features.Projects.Queries.GetAllProjects;

/// <summary>
/// Validates project list query parameters.
/// </summary>
public sealed class GetAllProjectsQueryValidator : AbstractValidator<GetAllProjectsQuery>
{
    private static readonly string[] SupportedSortFields = ["CreatedAt", "Name"];

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllProjectsQueryValidator" /> class.
    /// </summary>
    public GetAllProjectsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(PaginationParams.MinPageNumber);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(PaginationParams.MinPageSize, PaginationParams.MaxPageSize);

        RuleFor(query => query.Search)
            .MaximumLength(200);

        RuleFor(query => query.SortBy)
            .Must(sortBy => string.IsNullOrWhiteSpace(sortBy) ||
                SupportedSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            .WithMessage("SortBy must be either CreatedAt or Name.");
    }
}
