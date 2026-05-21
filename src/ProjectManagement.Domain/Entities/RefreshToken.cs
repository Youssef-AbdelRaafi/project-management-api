using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Domain.Entities;

/// <summary>
/// Refresh token lifecycle data for long-lived authentication sessions.
/// </summary>
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

    /// <summary>
    /// Gets the stored hash of the refresh token.
    /// </summary>
    public string TokenHash { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the UTC timestamp when the refresh token expires.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the refresh token was issued.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Gets the IP address that requested the refresh token.
    /// </summary>
    public string? CreatedByIp { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the refresh token was revoked.
    /// </summary>
    public DateTimeOffset? RevokedAt { get; private set; }

    /// <summary>
    /// Gets the IP address that revoked the refresh token.
    /// </summary>
    public string? RevokedByIp { get; private set; }

    /// <summary>
    /// Gets the hash of the replacement refresh token when token rotation occurs.
    /// </summary>
    public string? ReplacedByTokenHash { get; private set; }

    /// <summary>
    /// Gets the reason the refresh token was revoked.
    /// </summary>
    public string? RevokedReason { get; private set; }

    /// <summary>
    /// Gets the user identifier that owns the refresh token.
    /// </summary>
    public string UserId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the user that owns the refresh token.
    /// </summary>
    public ApplicationUser? User { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the token has been revoked.
    /// </summary>
    public bool IsRevoked => RevokedAt is not null;

    /// <summary>
    /// Creates a refresh token record.
    /// </summary>
    /// <param name="tokenHash">The hashed refresh token value.</param>
    /// <param name="expiresAt">The UTC expiry timestamp.</param>
    /// <param name="userId">The owner user identifier.</param>
    /// <param name="createdByIp">The issuing client IP address.</param>
    /// <returns>The created refresh token.</returns>
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

    /// <summary>
    /// Determines whether the refresh token is expired at a specific time.
    /// </summary>
    /// <param name="utcNow">The current UTC timestamp.</param>
    /// <returns><c>true</c> when the token is expired; otherwise, <c>false</c>.</returns>
    public bool IsExpired(DateTimeOffset utcNow)
    {
        return utcNow >= ExpiresAt;
    }

    /// <summary>
    /// Determines whether the refresh token can still be used.
    /// </summary>
    /// <param name="utcNow">The current UTC timestamp.</param>
    /// <returns><c>true</c> when the token is not expired or revoked; otherwise, <c>false</c>.</returns>
    public bool IsActive(DateTimeOffset utcNow)
    {
        return !IsRevoked && !IsExpired(utcNow);
    }

    /// <summary>
    /// Revokes the refresh token.
    /// </summary>
    /// <param name="revokedAt">The UTC revocation timestamp.</param>
    /// <param name="revokedByIp">The revoking client IP address.</param>
    /// <param name="replacedByTokenHash">The replacement token hash, if rotation occurred.</param>
    /// <param name="reason">The revocation reason.</param>
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
