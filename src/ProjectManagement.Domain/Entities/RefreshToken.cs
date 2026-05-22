using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    private RefreshToken()
    {
    }

    private RefreshToken(string tokenHash, DateTimeOffset expiresAt, string userId, string? createdByIp)
    {
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        UserId = userId;
        CreatedByIp = createdByIp;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public string TokenHash { get; private set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public string? CreatedByIp { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public string? RevokedByIp { get; private set; }

    public string? ReplacedByTokenHash { get; private set; }

    public string? RevokedReason { get; private set; }

    public string UserId { get; private set; } = string.Empty;

    public ApplicationUser? User { get; private set; }

    public bool IsRevoked => RevokedAt is not null;

    public static RefreshToken Create(
        string tokenHash,
        DateTimeOffset expiresAt,
        string userId,
        string? createdByIp)
    {
        ValidateTokenHash(tokenHash);
        ValidateExpiry(expiresAt);
        ValidateUserId(userId);
        ValidateIpAddress(createdByIp, nameof(createdByIp));

        return new RefreshToken(tokenHash.Trim(), expiresAt, userId.Trim(), NormalizeOptionalText(createdByIp));
    }

    public bool IsExpired(DateTimeOffset utcNow)
    {
        return utcNow >= ExpiresAt;
    }

    public bool IsActive(DateTimeOffset utcNow)
    {
        return !IsRevoked && !IsExpired(utcNow);
    }

    public void Revoke(
        DateTimeOffset revokedAt,
        string? revokedByIp,
        string? replacedByTokenHash,
        string? reason)
    {
        if (IsRevoked)
        {
            return;
        }

        ValidateIpAddress(revokedByIp, nameof(revokedByIp));
        ValidateOptionalTokenHash(replacedByTokenHash);
        ValidateRevokedReason(reason);

        RevokedAt = revokedAt;
        RevokedByIp = NormalizeOptionalText(revokedByIp);
        ReplacedByTokenHash = NormalizeOptionalText(replacedByTokenHash);
        RevokedReason = NormalizeOptionalText(reason);
    }

    private static void ValidateTokenHash(string tokenHash)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new DomainException("Refresh token hash is required.");
        }

        if (tokenHash.Length > DomainConstants.RefreshToken.TokenHashMaxLength)
        {
            throw new DomainException($"Refresh token hash cannot exceed {DomainConstants.RefreshToken.TokenHashMaxLength} characters.");
        }
    }

    private static void ValidateOptionalTokenHash(string? tokenHash)
    {
        if (tokenHash?.Length > DomainConstants.RefreshToken.TokenHashMaxLength)
        {
            throw new DomainException($"Replacement token hash cannot exceed {DomainConstants.RefreshToken.TokenHashMaxLength} characters.");
        }
    }

    private static void ValidateExpiry(DateTimeOffset expiresAt)
    {
        if (expiresAt <= DateTimeOffset.UtcNow)
        {
            throw new DomainException("Refresh token expiry must be in the future.");
        }
    }

    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new DomainException("Refresh token user is required.");
        }
    }

    private static void ValidateIpAddress(string? ipAddress, string parameterName)
    {
        if (ipAddress?.Length > DomainConstants.RefreshToken.IpAddressMaxLength)
        {
            throw new DomainException($"{parameterName} cannot exceed {DomainConstants.RefreshToken.IpAddressMaxLength} characters.");
        }
    }

    private static void ValidateRevokedReason(string? reason)
    {
        if (reason?.Length > DomainConstants.RefreshToken.RevokedReasonMaxLength)
        {
            throw new DomainException($"Refresh token revocation reason cannot exceed {DomainConstants.RefreshToken.RevokedReasonMaxLength} characters.");
        }
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
