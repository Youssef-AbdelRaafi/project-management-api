using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Tasks.DTOs;
using DomainTaskStatus = ProjectManagement.Domain.Enums.TaskStatus;

namespace ProjectManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

/// <summary>
/// Updates the workflow status of one task owned through its project.
/// </summary>
/// <param name="Id">The task identifier.</param>
/// <param name="Status">The new workflow status.</param>
public sealed record UpdateTaskStatusCommand(Guid Id, DomainTaskStatus Status)
    : IRequest<Result<TaskDto>>;
