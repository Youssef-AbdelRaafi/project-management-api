using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.DTOs;

namespace ProjectManagement.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Rotates a refresh token and issues a new authentication token pair.
/// </summary>
/// <param name="RefreshToken">The raw refresh token supplied by the client.</param>
public sealed record RefreshTokenCommand(string RefreshToken)
    : IRequest<Result<AuthResponseDto>>;
