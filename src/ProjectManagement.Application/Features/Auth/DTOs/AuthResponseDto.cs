namespace ProjectManagement.Application.Features.Auth.DTOs;

/// <summary>
/// Represents a successful authentication response.
/// </summary>
/// <param name="AccessToken">The JWT access token.</param>
/// <param name="RefreshToken">The raw refresh token returned once.</param>
/// <param name="ExpiresAt">The UTC access token expiration timestamp.</param>
/// <param name="User">The authenticated user information.</param>
public sealed record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAt,
    UserDto User);
