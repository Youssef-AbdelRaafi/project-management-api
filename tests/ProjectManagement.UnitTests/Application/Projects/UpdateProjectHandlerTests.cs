using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.Commands.UpdateProject;
using ProjectManagement.Application.Features.Projects.DTOs;
using ProjectManagement.Domain.Exceptions;
using ProjectManagement.UnitTests.Common;

namespace ProjectManagement.UnitTests.Application.Projects;

public sealed class UpdateProjectHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ExistingProjectOwnedByUser_UpdatesProject()
    {
        // Arrange
        var project = TestDataFactory.CreateProject();
        await AddAsync(project);

        var handler = new UpdateProjectCommandHandler(
            DbContext,
            CurrentUserServiceMock.Object,
            Mapper);

        var command = new UpdateProjectCommand(project.Id, "Updated Project", "Updated description");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(command.Name);
        result.Data.Description.Should().Be(command.Description);

        var persistedProject = await DbContext.Projects.SingleAsync(entity => entity.Id == project.Id);
        persistedProject.Name.Should().Be(command.Name);
        persistedProject.Description.Should().Be(command.Description);
    }

    [Fact]
    public async Task Handle_ProjectOwnedByAnotherUser_ThrowsForbiddenException()
    {
        // Arrange
        var project = TestDataFactory.CreateProject(TestDataFactory.OtherUserId);
        await AddAsync(project);

        var handler = new UpdateProjectCommandHandler(
            DbContext,
            CurrentUserServiceMock.Object,
            Mapper);

        var command = new UpdateProjectCommand(project.Id, "Updated Project", "Updated description");

        // Act
        Func<Task<Result<ProjectDto>>> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
