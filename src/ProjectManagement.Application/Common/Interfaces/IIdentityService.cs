using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Interfaces;

/// <summary>
/// Defines identity operations required by authentication handlers.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Registers a new application user.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="fullName">The user's display name.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created user wrapped in a result.</returns>
    Task<Result<ApplicationUser>> RegisterAsync(
        string email,
        string password,
        string fullName,
        CancellationToken cancellationToken);

    /// <summary>
    /// Validates credentials and returns the authenticated user.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The authenticated user wrapped in a result.</returns>
    Task<Result<ApplicationUser>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a user by identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user when found; otherwise, <c>null</c>.</returns>
    Task<ApplicationUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken);
}
