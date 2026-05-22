using MediatR;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.DTOs;
using DomainRefreshToken = ProjectManagement.Domain.Entities.RefreshToken;

namespace ProjectManagement.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IIdentityService identityService,
    IJwtService jwtService,
    IApplicationDbContext dbContext)
    : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var loginResult = await identityService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (!loginResult.IsSuccess || loginResult.Data is null)
        {
            return Result<AuthResponseDto>.Failure(
                loginResult.Error ?? "Invalid email or password.",
                loginResult.StatusCode);
        }

        var user = loginResult.Data;
        var roles = await identityService.GetUserRolesAsync(user, cancellationToken);
        var utcNow = DateTimeOffset.UtcNow;
        var accessToken = jwtService.GenerateAccessToken(user, roles);
        var accessTokenExpiresAt = jwtService.GetAccessTokenExpiration(utcNow);
        var refreshToken = jwtService.GenerateRefreshToken();
        var refreshTokenHash = jwtService.HashRefreshToken(refreshToken);
        var refreshTokenExpiresAt = jwtService.GetRefreshTokenExpiration(utcNow);

        dbContext.RefreshTokens.Add(DomainRefreshToken.Create(refreshTokenHash, refreshTokenExpiresAt, user.Id, null));
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<AuthResponseDto>.Success(
            new AuthResponseDto(
                accessToken,
                refreshToken,
                accessTokenExpiresAt,
                new UserDto(user.Id, user.Email ?? string.Empty, user.FullName)));
    }
}
