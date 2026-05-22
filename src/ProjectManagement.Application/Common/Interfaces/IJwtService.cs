using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(ApplicationUser user, IReadOnlyCollection<string> roles);

    DateTimeOffset GetAccessTokenExpiration(DateTimeOffset utcNow);

    string GenerateRefreshToken();

    DateTimeOffset GetRefreshTokenExpiration(DateTimeOffset utcNow);

    string HashRefreshToken(string refreshToken);

    bool ValidateRefreshToken(string refreshToken, RefreshToken storedRefreshToken, DateTimeOffset utcNow);
}
