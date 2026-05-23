using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Exceptions;
using DomainProject = ProjectManagement.Domain.Entities.Project;

namespace ProjectManagement.Application.Features.Projects.Commands.DeleteProject;

public sealed class DeleteProjectCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<DeleteProjectCommand, Result>
{
    public async Task<Result> Handle(
        DeleteProjectCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("Authenticated user is required.");
        }

        var project = await dbContext.Projects
            .Include(p => p.Tasks)
            .SingleOrDefaultAsync(project => project.Id == request.Id, cancellationToken);

        if (project is null)
        {
            throw new NotFoundException(nameof(DomainProject), request.Id);
        }

        if (project.UserId != userId)
        {
            throw new ForbiddenException("You are not allowed to delete this project.");
        }

        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(StatusCodes.NoContent);
    }
}
