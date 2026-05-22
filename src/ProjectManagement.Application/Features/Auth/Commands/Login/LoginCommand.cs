using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.DTOs;

namespace ProjectManagement.Application.Features.Auth.Commands.Login;

/// <summary>
/// Authenticates a user and issues an authentication token pair.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
public sealed record LoginCommand(string Email, string Password)
    : IRequest<Result<AuthResponseDto>>;
