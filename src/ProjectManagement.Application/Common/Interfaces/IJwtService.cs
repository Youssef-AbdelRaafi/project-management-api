using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(UserAccount user, IReadOnlyCollection<string> roles, DateTimeOffset utcNow);

    DateTimeOffset GetAccessTokenExpiration(DateTimeOffset utcNow);

    string GenerateRefreshToken();

    DateTimeOffset GetRefreshTokenExpiration(DateTimeOffset utcNow);

    string HashRefreshToken(string refreshToken);

    bool ValidateRefreshToken(string refreshToken, RefreshToken storedRefreshToken, DateTimeOffset utcNow);
}
