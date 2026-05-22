using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.Commands.Login;
using ProjectManagement.Application.Features.Auth.DTOs;
using ProjectManagement.Domain.Constants;
using ProjectManagement.Domain.Entities;
using ProjectManagement.UnitTests.Common;
using AppStatusCodes = ProjectManagement.Application.Common.Models.StatusCodes;

namespace ProjectManagement.UnitTests.Application.Auth;

public sealed class LoginCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ValidCredentials_ReturnsTokens()
    {
        // Arrange
        var user = TestDataFactory.CreateUser();
        var command = new LoginCommand(TestDataFactory.UserEmail, "Password1");

        IdentityServiceMock
            .Setup(service => service.LoginAsync(
                command.Email,
                command.Password,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ApplicationUser>.Success(user));

        IdentityServiceMock
            .Setup(service => service.GetUserRolesAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync([Roles.User]);

        var handler = new LoginCommandHandler(
            IdentityServiceMock.Object,
            JwtServiceMock.Object,
            DbContext);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(AppStatusCodes.Ok);
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("access-token");
        result.Data.RefreshToken.Should().Be("refresh-token");
        result.Data.User.Id.Should().Be(user.Id);
        result.Data.User.Email.Should().Be(TestDataFactory.UserEmail);

        var storedRefreshToken = await DbContext.RefreshTokens.SingleAsync();
        storedRefreshToken.TokenHash.Should().Be("hashed-refresh-token");
        storedRefreshToken.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ReturnsFailureResult()
    {
        // Arrange
        var command = new LoginCommand(TestDataFactory.UserEmail, "WrongPassword1");

        IdentityServiceMock
            .Setup(service => service.LoginAsync(
                command.Email,
                command.Password,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ApplicationUser>.Failure(
                "Invalid email or password.",
                AppStatusCodes.Unauthorized));

        var handler = new LoginCommandHandler(
            IdentityServiceMock.Object,
            JwtServiceMock.Object,
            DbContext);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(AppStatusCodes.Unauthorized);
        result.Error.Should().Be("Invalid email or password.");
        DbContext.RefreshTokens.Should().BeEmpty();

        JwtServiceMock.Verify(
            service => service.GenerateAccessToken(
                It.IsAny<ApplicationUser>(),
                It.IsAny<IReadOnlyCollection<string>>()),
            Times.Never);
    }
}
