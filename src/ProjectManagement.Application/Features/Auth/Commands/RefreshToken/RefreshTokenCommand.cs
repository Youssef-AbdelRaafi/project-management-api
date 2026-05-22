using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.DTOs;

namespace ProjectManagement.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken)
    : IRequest<Result<AuthResponseDto>>;
