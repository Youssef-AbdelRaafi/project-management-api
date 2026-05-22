using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.DTOs;

namespace ProjectManagement.Application.Features.Projects.Queries.GetProjectById;

public sealed record GetProjectByIdQuery(Guid Id) : IRequest<Result<ProjectDetailsDto>>;
