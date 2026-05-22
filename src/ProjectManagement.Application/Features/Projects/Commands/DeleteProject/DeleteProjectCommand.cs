using MediatR;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Features.Projects.Commands.DeleteProject;

/// <summary>
/// Deletes a project owned by the authenticated user.
/// </summary>
/// <param name="Id">The project identifier.</param>
public sealed record DeleteProjectCommand(Guid Id) : IRequest<Result>;
