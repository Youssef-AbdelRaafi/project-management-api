namespace ProjectManagement.Application.Features.Auth.DTOs;

/// <summary>
/// Represents authenticated user information returned to API clients.
/// </summary>
/// <param name="Id">The user identifier.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="FullName">The user's display name.</param>
public sealed record UserDto(string Id, string Email, string FullName);
