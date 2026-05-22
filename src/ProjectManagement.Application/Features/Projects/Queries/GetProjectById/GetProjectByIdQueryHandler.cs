using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.DTOs;
using ProjectManagement.Domain.Exceptions;
using DomainProject = ProjectManagement.Domain.Entities.Project;

namespace ProjectManagement.Application.Features.Projects.Queries.GetProjectById;

public sealed class GetProjectByIdQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<GetProjectByIdQuery, Result<ProjectDetailsDto>>
{
    public async Task<Result<ProjectDetailsDto>> Handle(
        GetProjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("Authenticated user is required.");
        }

        var project = await dbContext.Projects
            .AsNoTracking()
            .Include(project => project.Tasks)
            .SingleOrDefaultAsync(project => project.Id == request.Id, cancellationToken);

        if (project is null)
        {
            throw new NotFoundException(nameof(DomainProject), request.Id);
        }

        if (project.UserId != userId)
        {
            throw new ForbiddenException("You are not allowed to access this project.");
        }

        return Result<ProjectDetailsDto>.Success(mapper.Map<ProjectDetailsDto>(project));
    }
}
