namespace ProjectManagement.Infrastructure.Identity;

/// <summary>
/// Optional configuration for seeding a default administrator account.
/// </summary>
public sealed class DefaultAdminSettings
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "DefaultAdmin";

    /// <summary>
    /// Gets the default administrator email address.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Gets the default administrator password.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Gets the default administrator display name.
    /// </summary>
    public string? FullName { get; init; }

    /// <summary>
    /// Gets a value indicating whether the admin seed configuration is complete.
    /// </summary>
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password) &&
        !string.IsNullOrWhiteSpace(FullName);
}
