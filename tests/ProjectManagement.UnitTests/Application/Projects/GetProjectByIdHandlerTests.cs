using FluentAssertions;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Projects.DTOs;
using ProjectManagement.Application.Features.Projects.Queries.GetProjectById;
using ProjectManagement.Domain.Exceptions;
using ProjectManagement.UnitTests.Common;

namespace ProjectManagement.UnitTests.Application.Projects;

public sealed class GetProjectByIdHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ExistingProjectOwnedByUser_ReturnsProject()
    {
        // Arrange
        var project = TestDataFactory.CreateProject();
        await AddAsync(project);

        var handler = new GetProjectByIdQueryHandler(
            DbContext,
            CurrentUserServiceMock.Object,
            Mapper);

        var query = new GetProjectByIdQuery(project.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(project.Id);
        result.Data.Name.Should().Be(project.Name);
        result.Data.TasksCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_NonExistingProject_ThrowsNotFoundException()
    {
        // Arrange
        var handler = new GetProjectByIdQueryHandler(
            DbContext,
            CurrentUserServiceMock.Object,
            Mapper);

        var query = new GetProjectByIdQuery(Guid.NewGuid());

        // Act
        Func<Task<Result<ProjectDetailsDto>>> act = () => handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ProjectOwnedByAnotherUser_ThrowsForbiddenException()
    {
        // Arrange
        var project = TestDataFactory.CreateProject(TestDataFactory.OtherUserId);
        await AddAsync(project);

        var handler = new GetProjectByIdQueryHandler(
            DbContext,
            CurrentUserServiceMock.Object,
            Mapper);

        var query = new GetProjectByIdQuery(project.Id);

        // Act
        Func<Task<Result<ProjectDetailsDto>>> act = () => handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
