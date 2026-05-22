using MediatR;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Features.Projects.Commands.DeleteProject;

public sealed record DeleteProjectCommand(Guid Id) : IRequest<Result>;
