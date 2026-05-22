using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Tasks.DTOs;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Exceptions;
using DomainProject = ProjectManagement.Domain.Entities.Project;

namespace ProjectManagement.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// Handles task creation after validating project ownership.
/// </summary>
public sealed class CreateTaskCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    /// <inheritdoc />
    public async Task<Result<TaskDto>> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("Authenticated user is required.");
        }

        var project = await dbContext.Projects
            .SingleOrDefaultAsync(project => project.Id == request.ProjectId, cancellationToken);

        if (project is null)
        {
            throw new NotFoundException(nameof(DomainProject), request.ProjectId);
        }

        if (project.UserId != userId)
        {
            throw new ForbiddenException("You are not allowed to create tasks for this project.");
        }

        var task = TaskItem.Create(
            request.Title,
            request.Description,
            request.DueDate,
            request.Priority,
            project.Id);

        dbContext.TaskItems.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<TaskDto>.Success(mapper.Map<TaskDto>(task), StatusCodes.Created);
    }
}
