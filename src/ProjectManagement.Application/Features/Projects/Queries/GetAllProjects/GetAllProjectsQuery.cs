using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.DTOs;

namespace ProjectManagement.Application.Features.Projects.Queries.GetAllProjects;

/// <summary>
/// Gets paginated projects owned by the authenticated user.
/// </summary>
/// <param name="PageNumber">The requested page number.</param>
/// <param name="PageSize">The requested page size.</param>
/// <param name="Search">The optional project name search term.</param>
/// <param name="SortBy">The optional sort field. Supported values: CreatedAt, Name.</param>
/// <param name="SortDescending">A value indicating whether sorting should be descending.</param>
public sealed record GetAllProjectsQuery(
    int PageNumber = PaginationParams.MinPageNumber,
    int PageSize = PaginationParams.DefaultPageSize,
    string? Search = null,
    string? SortBy = null,
    bool SortDescending = true)
    : IRequest<Result<PaginatedList<ProjectDto>>>;
