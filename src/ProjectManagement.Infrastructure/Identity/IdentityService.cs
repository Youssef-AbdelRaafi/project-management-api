using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Constants;

namespace ProjectManagement.Infrastructure.Identity;

public sealed class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    public async Task<Result<UserAccount>> RegisterAsync(
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
            return Result<UserAccount>.ValidationFailure(
                identityResult.Errors.Select(error => error.Description).ToList());
        }

        var addRoleResult = await userManager.AddToRoleAsync(user, Roles.User);

        if (!addRoleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);

            return Result<UserAccount>.ValidationFailure(
                addRoleResult.Errors.Select(error => error.Description).ToList());
        }

        return Result<UserAccount>.Success(ToUserAccount(user), StatusCodes.Created);
    }

    public async Task<Result<UserAccount>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByEmailAsync(email);

        if (user is null || !await userManager.CheckPasswordAsync(user, password))
        {
            return Result<UserAccount>.Failure(InvalidCredentialsMessage, StatusCodes.Unauthorized);
        }

        return Result<UserAccount>.Success(ToUserAccount(user));
    }

    public async Task<UserAccount?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByIdAsync(userId);

        return user is null ? null : ToUserAccount(user);
    }

    public async Task<IReadOnlyCollection<string>> GetUserRolesAsync(
        UserAccount user,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var applicationUser = await userManager.FindByIdAsync(user.Id);

        if (applicationUser is null)
        {
            return [];
        }

        var roles = await userManager.GetRolesAsync(applicationUser);
        return roles.ToArray();
    }

    public async Task<IReadOnlyCollection<UserAccount>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var users = await userManager.Users
            .AsNoTracking()
            .Select(user => new UserAccount(user.Id, user.Email ?? string.Empty, user.FullName))
            .ToListAsync(cancellationToken);

        return users;
    }

    private static UserAccount ToUserAccount(ApplicationUser user)
    {
        return new UserAccount(user.Id, user.Email ?? string.Empty, user.FullName);
    }
}
