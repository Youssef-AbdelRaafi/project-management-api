using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Infrastructure.Identity;

public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Required]
    [MinLength(32)]
    public string Secret { get; init; } = string.Empty;

    [Range(1, 120)]
    public int AccessTokenExpiryMinutes { get; init; } = 15;

    [Range(1, 30)]
    public int RefreshTokenExpiryDays { get; init; } = 7;
}
