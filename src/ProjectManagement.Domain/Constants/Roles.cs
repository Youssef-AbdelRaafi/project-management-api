namespace ProjectManagement.Domain.Constants;

/// <summary>
/// Application role names used for authorization policies and seeded identity roles.
/// </summary>
public static class Roles
{
    /// <summary>
    /// Administrator role with elevated platform permissions.
    /// </summary>
    public const string Admin = "Admin";

    /// <summary>
    /// Standard authenticated application user role.
    /// </summary>
    public const string User = "User";

    /// <summary>
    /// Gets all supported application roles.
    /// </summary>
    public static readonly string[] All = [Admin, User];
}
