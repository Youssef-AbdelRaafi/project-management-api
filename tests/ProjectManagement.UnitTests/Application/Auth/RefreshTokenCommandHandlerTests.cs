using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Features.Auth.Commands.RefreshToken;
using ProjectManagement.Application.Features.Auth.DTOs;
using ProjectManagement.Domain.Constants;
using ProjectManagement.UnitTests.Common;
using AppStatusCodes = ProjectManagement.Application.Common.Models.StatusCodes;
using DomainRefreshToken = ProjectManagement.Domain.Entities.RefreshToken;

namespace ProjectManagement.UnitTests.Application.Auth;

public sealed class RefreshTokenCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_ValidRefreshToken_RotatesTokenAndReturnsNewPair()
    {
        // Arrange
        const string currentRefreshToken = "current-refresh-token";
        var user = TestDataFactory.CreateUser();
        var storedToken = DomainRefreshToken.Create(
            "hashed-current-refresh-token",
            DateTimeOffset.UtcNow.AddDays(1),
            TestDataFactory.UserId,
            null,
            DateTimeOffset.UtcNow);

        await AddAsync(storedToken);

        JwtServiceMock
            .Setup(service => service.ValidateRefreshToken(
                currentRefreshToken,
                storedToken,
                It.IsAny<DateTimeOffset>()))
            .Returns(true);

        IdentityServiceMock
            .Setup(service => service.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        IdentityServiceMock
            .Setup(service => service.GetUserRolesAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync([Roles.User]);

        var handler = new RefreshTokenCommandHandler(
            DbContext,
            IdentityServiceMock.Object,
            JwtServiceMock.Object);

        // Act
        var result = await handler.Handle(new RefreshTokenCommand(currentRefreshToken), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(AppStatusCodes.Ok);
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("access-token");
        result.Data.RefreshToken.Should().Be("refresh-token");

        storedToken.IsRevoked.Should().BeTrue();
        storedToken.ReplacedByTokenHash.Should().Be("hashed-refresh-token");

        var tokens = await DbContext.RefreshTokens.ToListAsync();
        tokens.Should().HaveCount(2);
        tokens.Should().Contain(token => token.TokenHash == "hashed-refresh-token" && token.UserId == user.Id);
    }

    [Fact]
    public async Task Handle_InvalidRefreshToken_ReturnsUnauthorizedFailure()
    {
        // Arrange
        var handler = new RefreshTokenCommandHandler(
            DbContext,
            IdentityServiceMock.Object,
            JwtServiceMock.Object);

        // Act
        var result = await handler.Handle(new RefreshTokenCommand("invalid-refresh-token"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(AppStatusCodes.Unauthorized);
        result.Error.Should().Be("Invalid refresh token.");
        DbContext.RefreshTokens.Should().BeEmpty();

        IdentityServiceMock.Verify(
            service => service.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
