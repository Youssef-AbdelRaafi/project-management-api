using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.DTOs;

namespace ProjectManagement.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(string Email, string Password, string FullName)
    : IRequest<Result<AuthResponseDto>>;
