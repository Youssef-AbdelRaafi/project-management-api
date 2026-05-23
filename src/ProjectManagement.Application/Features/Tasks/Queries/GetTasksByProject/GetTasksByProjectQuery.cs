using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Tasks.DTOs;
using DomainTaskStatus = ProjectManagement.Domain.Enums.TaskStatus;

namespace ProjectManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public sealed record GetTasksByProjectQuery(
    Guid ProjectId,
    DomainTaskStatus? Status = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<Result<PaginatedList<TaskDto>>>;
