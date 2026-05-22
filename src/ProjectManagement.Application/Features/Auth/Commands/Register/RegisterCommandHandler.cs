using MediatR;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.DTOs;
using DomainRefreshToken = ProjectManagement.Domain.Entities.RefreshToken;

namespace ProjectManagement.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IIdentityService identityService,
    IJwtService jwtService,
    IApplicationDbContext dbContext)
    : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var registrationResult = await identityService.RegisterAsync(
            request.Email,
            request.Password,
            request.FullName,
            cancellationToken);

        if (!registrationResult.IsSuccess || registrationResult.Data is null)
        {
            return Result<AuthResponseDto>.ValidationFailure(registrationResult.Errors);
        }

        var user = registrationResult.Data;
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
                new UserDto(user.Id, user.Email ?? string.Empty, user.FullName)),
            StatusCodes.Created);
    }
}
