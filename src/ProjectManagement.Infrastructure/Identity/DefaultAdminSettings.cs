namespace ProjectManagement.Infrastructure.Identity;

public sealed class DefaultAdminSettings
{
    public const string SectionName = "DefaultAdmin";

    public string? Email { get; init; }

    public string? Password { get; init; }

    public string? FullName { get; init; }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password) &&
        !string.IsNullOrWhiteSpace(FullName);
}
