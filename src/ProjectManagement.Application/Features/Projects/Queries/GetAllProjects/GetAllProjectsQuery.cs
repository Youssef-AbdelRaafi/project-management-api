using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.DTOs;

namespace ProjectManagement.Application.Features.Projects.Queries.GetAllProjects;

public sealed record GetAllProjectsQuery(
    int PageNumber = PaginationParams.MinPageNumber,
    int PageSize = PaginationParams.DefaultPageSize,
    string? Search = null,
    string? SortBy = null,
    bool SortDescending = true)
    : IRequest<Result<PaginatedList<ProjectDto>>>;
