using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Infrastructure.Identity;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public string? UserId => GetClaimValue(ClaimTypes.NameIdentifier)
        ?? GetClaimValue(JwtRegisteredClaimNames.Sub);

    public string? Email => GetClaimValue(ClaimTypes.Email)
        ?? GetClaimValue(JwtRegisteredClaimNames.Email);

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    private string? GetClaimValue(string claimType)
    {
        return User?.FindFirstValue(claimType);
    }
}
