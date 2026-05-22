using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Infrastructure.Identity;

/// <summary>
/// Configuration values used to issue and validate JWTs.
/// </summary>
public sealed class JwtSettings
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// Gets the JWT issuer.
    /// </summary>
    [Required]
    public string Issuer { get; init; } = string.Empty;

    /// <summary>
    /// Gets the JWT audience.
    /// </summary>
    [Required]
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    /// Gets the symmetric signing secret.
    /// </summary>
    [Required]
    [MinLength(32)]
    public string Secret { get; init; } = string.Empty;

    /// <summary>
    /// Gets the access token lifetime in minutes.
    /// </summary>
    [Range(1, 120)]
    public int AccessTokenExpiryMinutes { get; init; } = 15;

    /// <summary>
    /// Gets the refresh token lifetime in days.
    /// </summary>
    [Range(1, 30)]
    public int RefreshTokenExpiryDays { get; init; } = 7;
}
