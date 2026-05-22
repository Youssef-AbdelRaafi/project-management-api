using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Domain.Entities;

/// <summary>
/// A user-owned project containing task items.
/// </summary>
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

    /// <summary>
    /// Gets the project name.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the project description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the owner user identifier.
    /// </summary>
    public string UserId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the owner user.
    /// </summary>
    public ApplicationUser? User { get; private set; }

    /// <summary>
    /// Gets the tasks that belong to the project.
    /// </summary>
    public ICollection<TaskItem> Tasks { get; private set; } = new List<TaskItem>();

    /// <summary>
    /// Creates a new project for a user.
    /// </summary>
    /// <param name="name">The project name.</param>
    /// <param name="description">The project description.</param>
    /// <param name="userId">The owner user identifier.</param>
    /// <returns>The created project.</returns>
    public static Project Create(string name, string? description, string userId)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidateUserId(userId);

        return new Project(name.Trim(), NormalizeOptionalText(description), userId.Trim());
    }

    /// <summary>
    /// Updates the project's editable details.
    /// </summary>
    /// <param name="name">The project name.</param>
    /// <param name="description">The project description.</param>
    public void Update(string name, string? description)
    {
        ValidateName(name);
        ValidateDescription(description);

        Name = name.Trim();
        Description = NormalizeOptionalText(description);
    }

    /// <summary>
    /// Updates the project's editable details.
    /// </summary>
    /// <param name="name">The project name.</param>
    /// <param name="description">The project description.</param>
    public void UpdateDetails(string name, string? description)
    {
        Update(name, description);
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
