using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.DTOs;

namespace ProjectManagement.Application.Features.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(Guid Id, string Name, string? Description)
    : IRequest<Result<ProjectDto>>;
