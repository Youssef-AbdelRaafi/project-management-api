using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Domain.Entities;

/// <summary>
/// A task inside a project.
/// </summary>
public sealed class TaskItem : AuditableEntity
{
    private TaskItem()
    {
    }

    private TaskItem(
        string title,
        string? description,
        DateTime dueDate,
        TaskPriority priority,
        Guid projectId)
    {
        Title = title;
        Description = description;
        Status = Enums.TaskStatus.Todo;
        DueDate = dueDate;
        Priority = priority;
        ProjectId = projectId;
    }

    /// <summary>
    /// Gets the task title.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the task description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the workflow status.
    /// </summary>
    public Enums.TaskStatus Status { get; private set; } = Enums.TaskStatus.Todo;

    /// <summary>
    /// Gets the task due date.
    /// </summary>
    public DateTime DueDate { get; private set; }

    /// <summary>
    /// Gets the task priority.
    /// </summary>
    public TaskPriority Priority { get; private set; } = TaskPriority.Medium;

    /// <summary>
    /// Gets the parent project identifier.
    /// </summary>
    public Guid ProjectId { get; private set; }

    /// <summary>
    /// Gets the parent project.
    /// </summary>
    public Project? Project { get; private set; }

    /// <summary>
    /// Creates a new task in a project.
    /// </summary>
    /// <param name="title">The task title.</param>
    /// <param name="description">The task description.</param>
    /// <param name="dueDate">The task due date.</param>
    /// <param name="priority">The task priority.</param>
    /// <param name="projectId">The parent project identifier.</param>
    /// <returns>The created task.</returns>
    public static TaskItem Create(
        string title,
        string? description,
        DateTime dueDate,
        TaskPriority priority,
        Guid projectId)
    {
        ValidateTitle(title);
        ValidateDescription(description);
        ValidateProjectId(projectId);
        ValidatePriority(priority);

        return new TaskItem(title.Trim(), NormalizeOptionalText(description), dueDate, priority, projectId);
    }

    /// <summary>
    /// Updates the task status.
    /// </summary>
    /// <param name="status">The new task status.</param>
    public void UpdateStatus(Enums.TaskStatus status)
    {
        ValidateStatus(status);
        Status = status;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Task title is required.");
        }

        if (title.Length > DomainConstants.TaskItem.TitleMaxLength)
        {
            throw new DomainException($"Task title cannot exceed {DomainConstants.TaskItem.TitleMaxLength} characters.");
        }
    }

    private static void ValidateDescription(string? description)
    {
        if (description?.Length > DomainConstants.TaskItem.DescriptionMaxLength)
        {
            throw new DomainException($"Task description cannot exceed {DomainConstants.TaskItem.DescriptionMaxLength} characters.");
        }
    }

    private static void ValidateProjectId(Guid projectId)
    {
        if (projectId == Guid.Empty)
        {
            throw new DomainException("Task project is required.");
        }
    }

    private static void ValidatePriority(TaskPriority priority)
    {
        if (!Enum.IsDefined(priority))
        {
            throw new DomainException("Task priority is invalid.");
        }
    }

    private static void ValidateStatus(Enums.TaskStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new DomainException("Task status is invalid.");
        }
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
