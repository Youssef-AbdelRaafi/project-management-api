using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.DTOs;

namespace ProjectManagement.Application.Features.Projects.Queries.GetProjectById;

/// <summary>
/// Gets one project by identifier.
/// </summary>
/// <param name="Id">The project identifier.</param>
public sealed record GetProjectByIdQuery(Guid Id) : IRequest<Result<ProjectDetailsDto>>;
