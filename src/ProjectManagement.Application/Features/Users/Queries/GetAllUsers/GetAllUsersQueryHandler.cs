using MediatR;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.DTOs;

namespace ProjectManagement.Application.Features.Users.Queries.GetAllUsers;

internal sealed class GetAllUsersQueryHandler(IIdentityService identityService)
    : IRequestHandler<GetAllUsersQuery, Result<IReadOnlyCollection<UserDto>>>
{
    public async Task<Result<IReadOnlyCollection<UserDto>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await identityService.GetAllUsersAsync(cancellationToken);

        var userDtos = users
            .Select(user => new UserDto(user.Id, user.Email, user.FullName))
            .ToList();

        return Result<IReadOnlyCollection<UserDto>>.Success(userDtos);
    }
}
