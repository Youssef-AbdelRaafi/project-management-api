using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.Commands.Register;
using ProjectManagement.Domain.Constants;
using ProjectManagement.UnitTests.Common;
using AppStatusCodes = ProjectManagement.Application.Common.Models.StatusCodes;

namespace ProjectManagement.UnitTests.Application.Auth;

public sealed class RegisterCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsCreatedAuthResponse()
    {
        // Arrange
        var user = TestDataFactory.CreateUser();
        var command = new RegisterCommand(TestDataFactory.UserEmail, "Password1", "Test User");

        IdentityServiceMock
            .Setup(service => service.RegisterAsync(
                command.Email,
                command.Password,
                command.FullName,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserAccount>.Success(user, AppStatusCodes.Created));

        IdentityServiceMock
            .Setup(service => service.GetUserRolesAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync([Roles.User]);

        var handler = new RegisterCommandHandler(
            IdentityServiceMock.Object,
            JwtServiceMock.Object,
            DbContext);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(AppStatusCodes.Created);
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("access-token");
        result.Data.RefreshToken.Should().Be("refresh-token");
        result.Data.User.Id.Should().Be(user.Id);
        result.Data.User.FullName.Should().Be(command.FullName);

        var storedRefreshToken = await DbContext.RefreshTokens.SingleAsync();
        storedRefreshToken.TokenHash.Should().Be("hashed-refresh-token");
        storedRefreshToken.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Handle_IdentityValidationFailure_ReturnsValidationFailure()
    {
        // Arrange
        var command = new RegisterCommand(TestDataFactory.UserEmail, "Password1", "Test User");
        var errors = new List<string> { "Email is already registered." };

        IdentityServiceMock
            .Setup(service => service.RegisterAsync(
                command.Email,
                command.Password,
                command.FullName,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserAccount>.ValidationFailure(errors));

        var handler = new RegisterCommandHandler(
            IdentityServiceMock.Object,
            JwtServiceMock.Object,
            DbContext);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(AppStatusCodes.BadRequest);
        result.Errors.Should().BeEquivalentTo(errors);
        DbContext.RefreshTokens.Should().BeEmpty();

        JwtServiceMock.Verify(
            service => service.GenerateAccessToken(
                It.IsAny<UserAccount>(),
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<DateTimeOffset>()),
            Times.Never);
    }
}
