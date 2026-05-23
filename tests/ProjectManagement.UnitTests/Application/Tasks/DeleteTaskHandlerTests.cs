using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Tasks.Commands.DeleteTask;
using ProjectManagement.Domain.Exceptions;
using ProjectManagement.UnitTests.Common;
using AppStatusCodes = ProjectManagement.Application.Common.Models.StatusCodes;

namespace ProjectManagement.UnitTests.Application.Tasks;

public sealed class DeleteTaskHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_TaskInOwnedProject_DeletesTask()
    {
        // Arrange
        var project = TestDataFactory.CreateProject();
        var task = TestDataFactory.CreateTask(project.Id);
        await AddAsync(project);
        await AddAsync(task);

        var handler = new DeleteTaskCommandHandler(
            DbContext,
            CurrentUserServiceMock.Object);

        // Act
        var result = await handler.Handle(new DeleteTaskCommand(task.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(AppStatusCodes.NoContent);
        var deletedTask = await DbContext.TaskItems.IgnoreQueryFilters().SingleAsync(entity => entity.Id == task.Id);
        deletedTask.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_TaskInOtherUserProject_ThrowsForbiddenException()
    {
        // Arrange
        var project = TestDataFactory.CreateProject(TestDataFactory.OtherUserId);
        var task = TestDataFactory.CreateTask(project.Id);
        await AddAsync(project);
        await AddAsync(task);

        var handler = new DeleteTaskCommandHandler(
            DbContext,
            CurrentUserServiceMock.Object);

        // Act
        Func<Task<Result>> act = () => handler.Handle(new DeleteTaskCommand(task.Id), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
