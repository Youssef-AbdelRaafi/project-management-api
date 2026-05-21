namespace ProjectManagement.Domain.Enums;

/// <summary>
/// Defines the current workflow state of a task.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// The task has not started.
    /// </summary>
    Todo = 1,

    /// <summary>
    /// The task is actively being worked on.
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// The task is complete.
    /// </summary>
    Done = 3
}
