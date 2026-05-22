using AutoMapper;
using MediatR;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.DTOs;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Application.Features.Projects.Commands.CreateProject;

/// <summary>
/// Handles project creation for the authenticated user.
/// </summary>
public sealed class CreateProjectCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
{
    /// <inheritdoc />
    public async Task<Result<ProjectDto>> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("Authenticated user is required.");
        }

        var project = Project.Create(request.Name, request.Description, userId);

        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<ProjectDto>.Success(mapper.Map<ProjectDto>(project), StatusCodes.Created);
    }
}
