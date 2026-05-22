using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Tasks.DTOs;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Features.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    DateTime DueDate,
    TaskPriority Priority) : IRequest<Result<TaskDto>>;
