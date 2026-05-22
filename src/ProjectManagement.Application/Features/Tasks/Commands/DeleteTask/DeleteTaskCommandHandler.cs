using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Exceptions;
using DomainTaskItem = ProjectManagement.Domain.Entities.TaskItem;

namespace ProjectManagement.Application.Features.Tasks.Commands.DeleteTask;

public sealed class DeleteTaskCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<DeleteTaskCommand, Result>
{
    public async Task<Result> Handle(
        DeleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("Authenticated user is required.");
        }

        var task = await dbContext.TaskItems
            .Include(task => task.Project)
            .SingleOrDefaultAsync(task => task.Id == request.Id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException(nameof(DomainTaskItem), request.Id);
        }

        if (task.Project?.UserId != userId)
        {
            throw new ForbiddenException("You are not allowed to delete this task.");
        }

        dbContext.TaskItems.Remove(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(StatusCodes.NoContent);
    }
}
