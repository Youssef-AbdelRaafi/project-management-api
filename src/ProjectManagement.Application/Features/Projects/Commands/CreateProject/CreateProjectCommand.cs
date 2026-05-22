using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.DTOs;

namespace ProjectManagement.Application.Features.Projects.Commands.CreateProject;

/// <summary>
/// Creates a project for the authenticated user.
/// </summary>
/// <param name="Name">The project name.</param>
/// <param name="Description">The project description.</param>
public sealed record CreateProjectCommand(string Name, string? Description)
    : IRequest<Result<ProjectDto>>;
