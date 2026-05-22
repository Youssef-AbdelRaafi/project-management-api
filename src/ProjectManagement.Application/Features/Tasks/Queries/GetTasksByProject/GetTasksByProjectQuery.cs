using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Tasks.DTOs;
using DomainTaskStatus = ProjectManagement.Domain.Enums.TaskStatus;

namespace ProjectManagement.Application.Features.Tasks.Queries.GetTasksByProject;

/// <summary>
/// Gets tasks inside a project owned by the authenticated user.
/// </summary>
/// <param name="ProjectId">The parent project identifier.</param>
/// <param name="Status">Optional workflow status filter.</param>
public sealed record GetTasksByProjectQuery(Guid ProjectId, DomainTaskStatus? Status = null)
    : IRequest<Result<IReadOnlyCollection<TaskDto>>>;
