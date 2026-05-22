using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.DTOs;

namespace ProjectManagement.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password)
    : IRequest<Result<AuthResponseDto>>;
