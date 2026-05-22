using Microsoft.AspNetCore.Identity;
using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Infrastructure.Identity;

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

    public string FullName { get; private set; } = string.Empty;

    public ICollection<Project> Projects { get; private set; } = new List<Project>();

    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    public static ApplicationUser Create(string email, string fullName)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email is required.");
        }

        ValidateFullName(fullName);

        return new ApplicationUser(email.Trim(), fullName.Trim());
    }

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
