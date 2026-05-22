using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Tasks.DTOs;
using DomainTaskStatus = ProjectManagement.Domain.Enums.TaskStatus;

namespace ProjectManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public sealed record GetTasksByProjectQuery(Guid ProjectId, DomainTaskStatus? Status = null)
    : IRequest<Result<IReadOnlyCollection<TaskDto>>>;
