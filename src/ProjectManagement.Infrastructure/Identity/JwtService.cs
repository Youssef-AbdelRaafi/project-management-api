using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Identity;

/// <summary>
/// Issues JWT access tokens and manages refresh-token primitives.
/// </summary>
public sealed class JwtService(IOptions<JwtSettings> jwtOptions) : IJwtService
{
    private const int RefreshTokenByteLength = 64;

    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    /// <inheritdoc />
    public string GenerateAccessToken(ApplicationUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new("fullName", user.FullName)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc />
    public DateTimeOffset GetAccessTokenExpiration(DateTimeOffset utcNow)
    {
        return utcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
    }

    /// <inheritdoc />
    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(RefreshTokenByteLength);
        return Convert.ToBase64String(randomBytes);
    }

    /// <inheritdoc />
    public DateTimeOffset GetRefreshTokenExpiration(DateTimeOffset utcNow)
    {
        return utcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
    }

    /// <inheritdoc />
    public string HashRefreshToken(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ArgumentException("Refresh token is required.", nameof(refreshToken));
        }

        var tokenBytes = Encoding.UTF8.GetBytes(refreshToken);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToBase64String(hashBytes);
    }

    /// <inheritdoc />
    public bool ValidateRefreshToken(string refreshToken, RefreshToken storedRefreshToken, DateTimeOffset utcNow)
    {
        ArgumentNullException.ThrowIfNull(storedRefreshToken);

        if (string.IsNullOrWhiteSpace(refreshToken) || !storedRefreshToken.IsActive(utcNow))
        {
            return false;
        }

        var computedHash = HashRefreshToken(refreshToken);
        var computedHashBytes = Encoding.UTF8.GetBytes(computedHash);
        var storedHashBytes = Encoding.UTF8.GetBytes(storedRefreshToken.TokenHash);

        return computedHashBytes.Length == storedHashBytes.Length
            && CryptographicOperations.FixedTimeEquals(computedHashBytes, storedHashBytes);
    }
}
