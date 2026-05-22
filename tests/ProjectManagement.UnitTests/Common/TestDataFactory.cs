using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using DomainTaskStatus = ProjectManagement.Domain.Enums.TaskStatus;

namespace ProjectManagement.UnitTests.Common;

public static class TestDataFactory
{
    public const string UserId = "user-1";

    public const string OtherUserId = "user-2";

    public const string UserEmail = "owner@example.com";

    public static ApplicationUser CreateUser(
        string id = UserId,
        string email = UserEmail,
        string fullName = "Test User")
    {
        var user = ApplicationUser.Create(email, fullName);
        user.Id = id;

        return user;
    }

    public static Project CreateProject(
        string userId = UserId,
        string name = "Project Alpha",
        string? description = "Project description")
    {
        var project = Project.Create(name, description, userId);
        project.CreatedAt = DateTimeOffset.UtcNow;

        return project;
    }

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
