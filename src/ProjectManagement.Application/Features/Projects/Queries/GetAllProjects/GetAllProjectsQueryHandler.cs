using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.DTOs;
using ProjectManagement.Domain.Exceptions;
using DomainProject = ProjectManagement.Domain.Entities.Project;

namespace ProjectManagement.Application.Features.Projects.Queries.GetAllProjects;

/// <summary>
/// Handles paginated project list queries for the authenticated user.
/// </summary>
public sealed class GetAllProjectsQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<GetAllProjectsQuery, Result<PaginatedList<ProjectDto>>>
{
    /// <inheritdoc />
    public async Task<Result<PaginatedList<ProjectDto>>> Handle(
        GetAllProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("Authenticated user is required.");
        }

        var query = dbContext.Projects
            .AsNoTracking()
            .Where(project => project.UserId == userId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim();
            query = query.Where(project => project.Name.Contains(searchTerm));
        }

        query = ApplySorting(query, request.SortBy, request.SortDescending);

        var projects = await PaginatedList<ProjectDto>.CreateAsync(
            query.ProjectTo<ProjectDto>(mapper.ConfigurationProvider),
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result<PaginatedList<ProjectDto>>.Success(projects);
    }

    private static IQueryable<DomainProject> ApplySorting(
        IQueryable<DomainProject> query,
        string? sortBy,
        bool sortDescending)
    {
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "name" => sortDescending
                ? query.OrderByDescending(project => project.Name)
                : query.OrderBy(project => project.Name),

            _ => sortDescending
                ? query.OrderByDescending(project => project.CreatedAt)
                : query.OrderBy(project => project.CreatedAt)
        };
    }
}
