using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.DTOs;

namespace ProjectManagement.Application.Features.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(string Name, string? Description)
    : IRequest<Result<ProjectDto>>;
