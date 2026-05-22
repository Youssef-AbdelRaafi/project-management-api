using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Interfaces;

/// <summary>
/// Defines JWT and refresh-token operations used by authentication flows.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a short-lived access token for a user.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <param name="roles">The roles assigned to the authenticated user.</param>
    /// <returns>The generated JWT access token.</returns>
    string GenerateAccessToken(ApplicationUser user, IReadOnlyCollection<string> roles);

    /// <summary>
    /// Calculates when a newly issued access token expires.
    /// </summary>
    /// <param name="utcNow">The current UTC timestamp.</param>
    /// <returns>The access token expiration timestamp.</returns>
    DateTimeOffset GetAccessTokenExpiration(DateTimeOffset utcNow);

    /// <summary>
    /// Generates a cryptographically strong refresh token.
    /// </summary>
    /// <returns>The raw refresh token returned to the client once.</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Calculates when a newly issued refresh token expires.
    /// </summary>
    /// <param name="utcNow">The current UTC timestamp.</param>
    /// <returns>The refresh token expiration timestamp.</returns>
    DateTimeOffset GetRefreshTokenExpiration(DateTimeOffset utcNow);

    /// <summary>
    /// Hashes a raw refresh token before it is persisted.
    /// </summary>
    /// <param name="refreshToken">The raw refresh token.</param>
    /// <returns>The hashed refresh token.</returns>
    string HashRefreshToken(string refreshToken);

    /// <summary>
    /// Validates a raw refresh token against a stored token record.
    /// </summary>
    /// <param name="refreshToken">The raw refresh token supplied by the client.</param>
    /// <param name="storedRefreshToken">The stored refresh token record.</param>
    /// <param name="utcNow">The current UTC timestamp.</param>
    /// <returns><c>true</c> when the refresh token is valid; otherwise, <c>false</c>.</returns>
    bool ValidateRefreshToken(string refreshToken, RefreshToken storedRefreshToken, DateTimeOffset utcNow);
}
