namespace ProjectManagement.Domain.Common;

/// <summary>
/// Shared domain limits used by entities, validators, and EF Core configurations.
/// </summary>
public static class DomainConstants
{
    /// <summary>
    /// User-related domain limits.
    /// </summary>
    public static class User
    {
        /// <summary>
        /// Maximum length for a user's full name.
        /// </summary>
        public const int FullNameMaxLength = 200;
    }

    /// <summary>
    /// Project-related domain limits.
    /// </summary>
    public static class Project
    {
        /// <summary>
        /// Maximum length for a project name.
        /// </summary>
        public const int NameMaxLength = 200;

        /// <summary>
        /// Maximum length for a project description.
        /// </summary>
        public const int DescriptionMaxLength = 1000;
    }

    /// <summary>
    /// Task-related domain limits.
    /// </summary>
    public static class TaskItem
    {
        /// <summary>
        /// Maximum length for a task title.
        /// </summary>
        public const int TitleMaxLength = 200;

        /// <summary>
        /// Maximum length for a task description.
        /// </summary>
        public const int DescriptionMaxLength = 1000;
    }

    /// <summary>
    /// Refresh-token-related domain limits.
    /// </summary>
    public static class RefreshToken
    {
        /// <summary>
        /// Maximum length for a stored refresh token hash.
        /// </summary>
        public const int TokenHashMaxLength = 128;

        /// <summary>
        /// Maximum length for an IP address string.
        /// </summary>
        public const int IpAddressMaxLength = 45;

        /// <summary>
        /// Maximum length for token revocation reasons.
        /// </summary>
        public const int RevokedReasonMaxLength = 500;
    }
}
