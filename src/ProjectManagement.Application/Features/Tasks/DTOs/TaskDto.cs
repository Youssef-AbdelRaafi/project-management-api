using ProjectManagement.Domain.Enums;
using DomainTaskStatus = ProjectManagement.Domain.Enums.TaskStatus;

namespace ProjectManagement.Application.Features.Tasks.DTOs;

/// <summary>
/// Task data returned by task endpoints.
/// </summary>
/// <param name="Id">The task identifier.</param>
/// <param name="Title">The task title.</param>
/// <param name="Description">The task description.</param>
/// <param name="Status">The current workflow status.</param>
/// <param name="DueDate">The task due date.</param>
/// <param name="Priority">The task priority.</param>
/// <param name="ProjectId">The parent project identifier.</param>
/// <param name="CreatedAt">The task creation timestamp.</param>
public sealed record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    DomainTaskStatus Status,
    DateTime DueDate,
    TaskPriority Priority,
    Guid ProjectId,
    DateTimeOffset CreatedAt);
