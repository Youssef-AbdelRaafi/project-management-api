using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Tasks.DTOs;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// Creates a task inside a project owned by the authenticated user.
/// </summary>
/// <param name="ProjectId">The parent project identifier.</param>
/// <param name="Title">The task title.</param>
/// <param name="Description">The task description.</param>
/// <param name="DueDate">The task due date.</param>
/// <param name="Priority">The task priority.</param>
public sealed record CreateTaskCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    DateTime DueDate,
    TaskPriority Priority) : IRequest<Result<TaskDto>>;
