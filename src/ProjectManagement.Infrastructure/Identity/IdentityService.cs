using Microsoft.AspNetCore.Identity;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Constants;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Identity;

/// <summary>
/// Wraps ASP.NET Core Identity operations behind application-level contracts.
/// </summary>
public sealed class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    /// <inheritdoc />
    public async Task<Result<ApplicationUser>> RegisterAsync(
        string email,
        string password,
        string fullName,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = ApplicationUser.Create(email, fullName);
        var identityResult = await userManager.CreateAsync(user, password);

        if (!identityResult.Succeeded)
        {
            return Result<ApplicationUser>.ValidationFailure(
                identityResult.Errors.Select(error => error.Description).ToList());
        }

        var addRoleResult = await userManager.AddToRoleAsync(user, Roles.User);

        if (!addRoleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);

            return Result<ApplicationUser>.ValidationFailure(
                addRoleResult.Errors.Select(error => error.Description).ToList());
        }

        return Result<ApplicationUser>.Success(user, StatusCodes.Created);
    }

    /// <inheritdoc />
    public async Task<Result<ApplicationUser>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByEmailAsync(email);

        if (user is null || !await userManager.CheckPasswordAsync(user, password))
        {
            return Result<ApplicationUser>.Failure(InvalidCredentialsMessage, StatusCodes.Unauthorized);
        }

        return Result<ApplicationUser>.Success(user);
    }

    /// <inheritdoc />
    public async Task<ApplicationUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await userManager.FindByIdAsync(userId);
    }
}
