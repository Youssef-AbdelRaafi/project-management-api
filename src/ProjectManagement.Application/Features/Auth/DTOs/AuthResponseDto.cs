namespace ProjectManagement.Application.Features.Auth.DTOs;

public sealed record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAt,
    UserDto User);
