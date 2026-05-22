using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Domain.Entities;

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

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public Enums.TaskStatus Status { get; private set; } = Enums.TaskStatus.Todo;

    public DateTime DueDate { get; private set; }

    public TaskPriority Priority { get; private set; } = TaskPriority.Medium;

    public Guid ProjectId { get; private set; }

    public Project? Project { get; private set; }

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
