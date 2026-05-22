using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Tasks.Commands.CreateTask;
using ProjectManagement.Application.Features.Tasks.DTOs;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Exceptions;
using ProjectManagement.UnitTests.Common;
using AppStatusCodes = ProjectManagement.Application.Common.Models.StatusCodes;
using DomainTaskStatus = ProjectManagement.Domain.Enums.TaskStatus;

namespace ProjectManagement.UnitTests.Application.Tasks;

public sealed class CreateTaskHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ValidTaskInOwnedProject_ReturnsSuccess()
    {
        // Arrange
        var project = TestDataFactory.CreateProject();
        await AddAsync(project);

        var handler = new CreateTaskCommandHandler(
            DbContext,
            CurrentUserServiceMock.Object,
            Mapper);

        var command = new CreateTaskCommand(
            project.Id,
            "Build task tests",
            "Cover ownership through project",
            DateTime.UtcNow.AddDays(5),
            TaskPriority.High);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(AppStatusCodes.Created);
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be(command.Title);
        result.Data.ProjectId.Should().Be(project.Id);
        result.Data.Status.Should().Be(DomainTaskStatus.Todo);
        result.Data.Priority.Should().Be(TaskPriority.High);

        var persistedTask = await DbContext.TaskItems.SingleAsync();
        persistedTask.ProjectId.Should().Be(project.Id);
        persistedTask.Title.Should().Be(command.Title);
    }

    [Fact]
    public async Task Handle_TaskInNonExistentProject_ThrowsNotFound()
    {
        // Arrange
        var handler = new CreateTaskCommandHandler(
            DbContext,
            CurrentUserServiceMock.Object,
            Mapper);

        var command = new CreateTaskCommand(
            Guid.NewGuid(),
            "Build task tests",
            "Cover ownership through project",
            DateTime.UtcNow.AddDays(5),
            TaskPriority.High);

        // Act
        Func<Task<Result<TaskDto>>> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_TaskInOtherUserProject_ThrowsForbidden()
    {
        // Arrange
        var project = TestDataFactory.CreateProject(TestDataFactory.OtherUserId);
        await AddAsync(project);

        var handler = new CreateTaskCommandHandler(
            DbContext,
            CurrentUserServiceMock.Object,
            Mapper);

        var command = new CreateTaskCommand(
            project.Id,
            "Build task tests",
            "Cover ownership through project",
            DateTime.UtcNow.AddDays(5),
            TaskPriority.High);

        // Act
        Func<Task<Result<TaskDto>>> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
