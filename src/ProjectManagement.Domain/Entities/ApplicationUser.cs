using Microsoft.AspNetCore.Identity;
using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Domain.Entities;

/// <summary>
/// Application identity user that owns projects and refresh tokens.
/// </summary>
public sealed class ApplicationUser : IdentityUser
{
    private ApplicationUser()
    {
    }

    private ApplicationUser(string email, string fullName)
    {
        Email = email;
        UserName = email;
        FullName = fullName;
    }

    /// <summary>
    /// Gets the user's display name.
    /// </summary>
    public string FullName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the projects owned by the user.
    /// </summary>
    public ICollection<Project> Projects { get; private set; } = new List<Project>();

    /// <summary>
    /// Gets the refresh tokens issued to the user.
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    /// <summary>
    /// Creates a new application user.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="fullName">The user's display name.</param>
    /// <returns>The created application user.</returns>
    public static ApplicationUser Create(string email, string fullName)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email is required.");
        }

        ValidateFullName(fullName);

        return new ApplicationUser(email.Trim(), fullName.Trim());
    }

    /// <summary>
    /// Updates the user's display name.
    /// </summary>
    /// <param name="fullName">The new display name.</param>
    public void UpdateFullName(string fullName)
    {
        ValidateFullName(fullName);
        FullName = fullName.Trim();
    }

    private static void ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainException("Full name is required.");
        }

        if (fullName.Length > DomainConstants.User.FullNameMaxLength)
        {
            throw new DomainException($"Full name cannot exceed {DomainConstants.User.FullNameMaxLength} characters.");
        }
    }
}
