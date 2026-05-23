using MediatR;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Result>;
