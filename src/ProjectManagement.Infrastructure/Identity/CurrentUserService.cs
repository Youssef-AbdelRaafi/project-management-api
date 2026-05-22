using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Infrastructure.Identity;

/// <summary>
/// Reads the authenticated user's claims from the current HTTP context.
/// </summary>
public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    /// <inheritdoc />
    public string? UserId => GetClaimValue(ClaimTypes.NameIdentifier)
        ?? GetClaimValue(JwtRegisteredClaimNames.Sub);

    /// <inheritdoc />
    public string? Email => GetClaimValue(ClaimTypes.Email)
        ?? GetClaimValue(JwtRegisteredClaimNames.Email);

    /// <inheritdoc />
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    private string? GetClaimValue(string claimType)
    {
        return User?.FindFirstValue(claimType);
    }
}
