using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Tasks.DTOs;
using ProjectManagement.Domain.Exceptions;
using DomainProject = ProjectManagement.Domain.Entities.Project;

namespace ProjectManagement.Application.Features.Tasks.Queries.GetTasksByProject;

/// <summary>
/// Handles task list queries after validating project ownership.
/// </summary>
public sealed class GetTasksByProjectQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<GetTasksByProjectQuery, Result<IReadOnlyCollection<TaskDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyCollection<TaskDto>>> Handle(
        GetTasksByProjectQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("Authenticated user is required.");
        }

        var project = await dbContext.Projects
            .AsNoTracking()
            .SingleOrDefaultAsync(project => project.Id == request.ProjectId, cancellationToken);

        if (project is null)
        {
            throw new NotFoundException(nameof(DomainProject), request.ProjectId);
        }

        if (project.UserId != userId)
        {
            throw new ForbiddenException("You are not allowed to access tasks for this project.");
        }

        var query = dbContext.TaskItems
            .AsNoTracking()
            .Where(task => task.ProjectId == request.ProjectId);

        if (request.Status.HasValue)
        {
            query = query.Where(task => task.Status == request.Status.Value);
        }

        var tasks = await query
            .OrderBy(task => task.CreatedAt)
            .ProjectTo<TaskDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyCollection<TaskDto>>.Success(tasks);
    }
}
