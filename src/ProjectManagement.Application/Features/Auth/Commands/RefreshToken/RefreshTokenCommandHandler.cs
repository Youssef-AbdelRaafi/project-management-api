using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.DTOs;
using DomainRefreshToken = ProjectManagement.Domain.Entities.RefreshToken;

namespace ProjectManagement.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IApplicationDbContext dbContext,
    IIdentityService identityService,
    IJwtService jwtService)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private const string InvalidRefreshTokenMessage = "Invalid refresh token.";
    private const string RotationReason = "Rotated by refresh token flow.";

    public async Task<Result<AuthResponseDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var utcNow = DateTimeOffset.UtcNow;
        var refreshTokenHash = jwtService.HashRefreshToken(request.RefreshToken);
        var storedRefreshToken = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(token => token.TokenHash == refreshTokenHash, cancellationToken);

        if (storedRefreshToken is null ||
            !jwtService.ValidateRefreshToken(request.RefreshToken, storedRefreshToken, utcNow))
        {
            return Result<AuthResponseDto>.Failure(InvalidRefreshTokenMessage, StatusCodes.Unauthorized);
        }

        var user = await identityService.GetUserByIdAsync(storedRefreshToken.UserId, cancellationToken);

        if (user is null)
        {
            return Result<AuthResponseDto>.Failure(InvalidRefreshTokenMessage, StatusCodes.Unauthorized);
        }

        var newRefreshToken = jwtService.GenerateRefreshToken();
        var newRefreshTokenHash = jwtService.HashRefreshToken(newRefreshToken);
        var newRefreshTokenExpiresAt = jwtService.GetRefreshTokenExpiration(utcNow);

        storedRefreshToken.Revoke(utcNow, null, newRefreshTokenHash, RotationReason);
        dbContext.RefreshTokens.Add(DomainRefreshToken.Create(
            newRefreshTokenHash,
            newRefreshTokenExpiresAt,
            user.Id,
            null));

        await dbContext.SaveChangesAsync(cancellationToken);

        var roles = await identityService.GetUserRolesAsync(user, cancellationToken);
        var accessToken = jwtService.GenerateAccessToken(user, roles);
        var accessTokenExpiresAt = jwtService.GetAccessTokenExpiration(utcNow);

        return Result<AuthResponseDto>.Success(
            new AuthResponseDto(
                accessToken,
                newRefreshToken,
                accessTokenExpiresAt,
                new UserDto(user.Id, user.Email ?? string.Empty, user.FullName)));
    }
}
