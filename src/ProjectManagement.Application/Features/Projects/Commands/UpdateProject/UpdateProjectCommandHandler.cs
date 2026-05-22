using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.DTOs;
using ProjectManagement.Domain.Exceptions;
using DomainProject = ProjectManagement.Domain.Entities.Project;

namespace ProjectManagement.Application.Features.Projects.Commands.UpdateProject;

/// <summary>
/// Handles project updates for the authenticated owner.
/// </summary>
public sealed class UpdateProjectCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
{
    /// <inheritdoc />
    public async Task<Result<ProjectDto>> Handle(
        UpdateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("Authenticated user is required.");
        }

        var project = await dbContext.Projects
            .SingleOrDefaultAsync(project => project.Id == request.Id, cancellationToken);

        if (project is null)
        {
            throw new NotFoundException(nameof(DomainProject), request.Id);
        }

        if (project.UserId != userId)
        {
            throw new ForbiddenException("You are not allowed to update this project.");
        }

        project.Update(request.Name, request.Description);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<ProjectDto>.Success(mapper.Map<ProjectDto>(project));
    }
}
