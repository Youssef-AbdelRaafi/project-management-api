using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Tasks.DTOs;
using ProjectManagement.Domain.Exceptions;
using DomainTaskItem = ProjectManagement.Domain.Entities.TaskItem;

namespace ProjectManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

/// <summary>
/// Handles task status updates after validating project ownership.
/// </summary>
public sealed class UpdateTaskStatusCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<UpdateTaskStatusCommand, Result<TaskDto>>
{
    /// <inheritdoc />
    public async Task<Result<TaskDto>> Handle(
        UpdateTaskStatusCommand request,
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
            throw new ForbiddenException("You are not allowed to update this task.");
        }

        task.UpdateStatus(request.Status);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<TaskDto>.Success(mapper.Map<TaskDto>(task));
    }
}
