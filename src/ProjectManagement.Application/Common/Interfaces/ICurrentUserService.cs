namespace ProjectManagement.Application.Common.Interfaces;

/// <summary>
/// Provides information about the authenticated user for the current request.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's identifier.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the current user's email address.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets a value indicating whether the current request is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}
