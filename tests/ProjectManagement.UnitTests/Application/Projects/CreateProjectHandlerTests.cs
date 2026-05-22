using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.Commands.CreateProject;
using ProjectManagement.Domain.Exceptions;
using ProjectManagement.UnitTests.Common;
using AppStatusCodes = ProjectManagement.Application.Common.Models.StatusCodes;

namespace ProjectManagement.UnitTests.Application.Projects;

public sealed class CreateProjectHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessWithProjectDto()
    {
        // Arrange
        var handler = new CreateProjectCommandHandler(
            DbContext,
            CurrentUserServiceMock.Object,
            Mapper);

        var command = new CreateProjectCommand("Assessment API", "Clean architecture sample");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(AppStatusCodes.Created);
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(command.Name);
        result.Data.Description.Should().Be(command.Description);

        var persistedProject = await DbContext.Projects.SingleAsync();
        persistedProject.UserId.Should().Be(TestDataFactory.UserId);
        persistedProject.Name.Should().Be(command.Name);
    }

    [Fact]
    public async Task Handle_UnauthenticatedUser_ThrowsUnauthorized()
    {
        // Arrange
        SetCurrentUser(null, isAuthenticated: false);
        var handler = new CreateProjectCommandHandler(
            DbContext,
            CurrentUserServiceMock.Object,
            Mapper);

        var command = new CreateProjectCommand("Assessment API", "Clean architecture sample");

        // Act
        Func<Task<Result<ProjectManagement.Application.Features.Projects.DTOs.ProjectDto>>> act =
            () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
