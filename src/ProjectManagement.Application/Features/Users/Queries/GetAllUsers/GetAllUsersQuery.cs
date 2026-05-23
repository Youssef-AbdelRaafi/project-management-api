using MediatR;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.DTOs;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Application.Features.Users.Queries.GetAllUsers;

public sealed record GetAllUsersQuery : IRequest<Result<IReadOnlyCollection<UserDto>>>;
