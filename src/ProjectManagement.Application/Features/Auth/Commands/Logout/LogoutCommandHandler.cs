using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler(
    IApplicationDbContext dbContext,
    IJwtService jwtService,
    ICurrentUserService currentUserService)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure("User is not authenticated.", StatusCodes.Unauthorized);
        }

        var tokenHash = jwtService.HashRefreshToken(request.RefreshToken);

        var storedToken = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(t => t.TokenHash == tokenHash && t.UserId == userId, cancellationToken);

        if (storedToken is null)
        {
            return Result.Failure("Invalid refresh token.", StatusCodes.BadRequest);
        }

        if (!storedToken.IsRevoked)
        {
            storedToken.Revoke(DateTimeOffset.UtcNow, null, null, "User explicitly logged out.");
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
