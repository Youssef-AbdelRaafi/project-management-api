namespace ProjectManagement.Domain.Common;

public static class DomainConstants
{
    public static class User
    {
        public const int FullNameMaxLength = 200;
    }

    public static class Project
    {
        public const int NameMaxLength = 200;

        public const int DescriptionMaxLength = 1000;
    }

    public static class TaskItem
    {
        public const int TitleMaxLength = 200;

        public const int DescriptionMaxLength = 1000;
    }

    public static class RefreshToken
    {
        public const int TokenHashMaxLength = 128;

        public const int IpAddressMaxLength = 45;

        public const int RevokedReasonMaxLength = 500;
    }
}
