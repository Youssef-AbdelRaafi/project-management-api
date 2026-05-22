using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.DTOs;

namespace ProjectManagement.Application.Features.Projects.Commands.UpdateProject;

/// <summary>
/// Updates a project owned by the authenticated user.
/// </summary>
/// <param name="Id">The project identifier.</param>
/// <param name="Name">The project name.</param>
/// <param name="Description">The project description.</param>
public sealed record UpdateProjectCommand(Guid Id, string Name, string? Description)
    : IRequest<Result<ProjectDto>>;
