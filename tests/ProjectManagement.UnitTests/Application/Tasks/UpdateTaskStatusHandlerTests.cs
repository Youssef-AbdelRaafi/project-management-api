using FluentAssertions;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using ProjectManagement.Application.Features.Tasks.DTOs;
using ProjectManagement.Domain.Exceptions;
using ProjectManagement.UnitTests.Common;
using DomainTaskStatus = ProjectManagement.Domain.Enums.TaskStatus;

namespace ProjectManagement.UnitTests.Application.Tasks;

public sealed class UpdateTaskStatusHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_TaskInOwnedProject_UpdatesStatus()
    {
        // Arrange
        var project = TestDataFactory.CreateProject();
        var task = TestDataFactory.CreateTask(project.Id);
        await AddAsync(project);
        await AddAsync(task);

        var handler = new UpdateTaskStatusCommandHandler(
            DbContext,
            CurrentUserServiceMock.Object,
            Mapper);

        var command = new UpdateTaskStatusCommand(task.Id, DomainTaskStatus.InProgress);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Status.Should().Be(DomainTaskStatus.InProgress);
        task.Status.Should().Be(DomainTaskStatus.InProgress);
    }

    [Fact]
    public async Task Handle_TaskInOtherUserProject_ThrowsForbiddenException()
    {
        // Arrange
        var project = TestDataFactory.CreateProject(TestDataFactory.OtherUserId);
        var task = TestDataFactory.CreateTask(project.Id);
        await AddAsync(project);
        await AddAsync(task);

        var handler = new UpdateTaskStatusCommandHandler(
            DbContext,
            CurrentUserServiceMock.Object,
            Mapper);

        var command = new UpdateTaskStatusCommand(task.Id, DomainTaskStatus.Done);

        // Act
        Func<Task<Result<TaskDto>>> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
