using MediatR;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Features.Tasks.Commands.DeleteTask;

/// <summary>
/// Deletes one task owned through its project.
/// </summary>
/// <param name="Id">The task identifier.</param>
public sealed record DeleteTaskCommand(Guid Id) : IRequest<Result>;
