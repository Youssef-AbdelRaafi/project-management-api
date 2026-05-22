using ProjectManagement.Domain.Enums;
using DomainTaskStatus = ProjectManagement.Domain.Enums.TaskStatus;

namespace ProjectManagement.Application.Features.Tasks.DTOs;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    DomainTaskStatus Status,
    DateTime DueDate,
    TaskPriority Priority,
    Guid ProjectId,
    DateTimeOffset CreatedAt);
