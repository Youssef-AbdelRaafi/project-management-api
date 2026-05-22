using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.DTOs;

namespace ProjectManagement.Application.Features.Auth.Commands.Register;

/// <summary>
/// Registers a new user and issues an authentication token pair.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
/// <param name="FullName">The user's display name.</param>
public sealed record RegisterCommand(string Email, string Password, string FullName)
    : IRequest<Result<AuthResponseDto>>;
