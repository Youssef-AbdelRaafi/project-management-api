using MediatR;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Features.Tasks.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid Id) : IRequest<Result>;
