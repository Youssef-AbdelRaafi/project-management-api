using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Domain.Entities;

public sealed class Project : AuditableEntity
{
    private Project()
    {
    }

    private Project(string name, string? description, string userId)
    {
        Name = name;
        Description = description;
        UserId = userId;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string UserId { get; private set; } = string.Empty;

    public ICollection<TaskItem> Tasks { get; private set; } = new List<TaskItem>();

    public static Project Create(string name, string? description, string userId)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidateUserId(userId);

        return new Project(name.Trim(), NormalizeOptionalText(description), userId.Trim());
    }

    public void Update(string name, string? description)
    {
        ValidateName(name);
        ValidateDescription(description);

        Name = name.Trim();
        Description = NormalizeOptionalText(description);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Project name is required.");
        }

        if (name.Length > DomainConstants.Project.NameMaxLength)
        {
            throw new DomainException($"Project name cannot exceed {DomainConstants.Project.NameMaxLength} characters.");
        }
    }

    private static void ValidateDescription(string? description)
    {
        if (description?.Length > DomainConstants.Project.DescriptionMaxLength)
        {
            throw new DomainException($"Project description cannot exceed {DomainConstants.Project.DescriptionMaxLength} characters.");
        }
    }

    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new DomainException("Project owner is required.");
        }
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
