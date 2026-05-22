using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Tasks.DTOs;
using DomainTaskStatus = ProjectManagement.Domain.Enums.TaskStatus;

namespace ProjectManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

public sealed record UpdateTaskStatusCommand(Guid Id, DomainTaskStatus Status)
    : IRequest<Result<TaskDto>>;
