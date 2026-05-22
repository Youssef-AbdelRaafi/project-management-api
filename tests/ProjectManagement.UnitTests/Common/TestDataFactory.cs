using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using DomainTaskStatus = ProjectManagement.Domain.Enums.TaskStatus;

namespace ProjectManagement.UnitTests.Common;

/// <summary>
/// Factory methods for creating valid domain entities in tests.
/// </summary>
public static class TestDataFactory
{
    /// <summary>
    /// The default test user identifier.
    /// </summary>
    public const string UserId = "user-1";

    /// <summary>
    /// The default second-user identifier.
    /// </summary>
    public const string OtherUserId = "user-2";

    /// <summary>
    /// The default test user email.
    /// </summary>
    public const string UserEmail = "owner@example.com";

    /// <summary>
    /// Creates a valid application user.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="email">The user email.</param>
    /// <param name="fullName">The user's full name.</param>
    /// <returns>The created application user.</returns>
    public static ApplicationUser CreateUser(
        string id = UserId,
        string email = UserEmail,
        string fullName = "Test User")
    {
        var user = ApplicationUser.Create(email, fullName);
        user.Id = id;

        return user;
    }

    /// <summary>
    /// Creates a valid project.
    /// </summary>
    /// <param name="userId">The owner user identifier.</param>
    /// <param name="name">The project name.</param>
    /// <param name="description">The project description.</param>
    /// <returns>The created project.</returns>
    public static Project CreateProject(
        string userId = UserId,
        string name = "Project Alpha",
        string? description = "Project description")
    {
        var project = Project.Create(name, description, userId);
        project.CreatedAt = DateTimeOffset.UtcNow;

        return project;
    }

    /// <summary>
    /// Creates a valid task item.
    /// </summary>
    /// <param name="projectId">The parent project identifier.</param>
    /// <param name="title">The task title.</param>
    /// <param name="status">The initial task status.</param>
    /// <returns>The created task item.</returns>
    public static TaskItem CreateTask(
        Guid projectId,
        string title = "Prepare API tests",
        DomainTaskStatus status = DomainTaskStatus.Todo)
    {
        var task = TaskItem.Create(
            title,
            "Task description",
            DateTime.UtcNow.AddDays(3),
            TaskPriority.Medium,
            projectId);

        task.CreatedAt = DateTimeOffset.UtcNow;

        if (status != DomainTaskStatus.Todo)
        {
            task.UpdateStatus(status);
        }

        return task;
    }
}
