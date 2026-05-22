using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Persistence;

namespace ProjectManagement.UnitTests.Common;

/// <summary>
/// Shared unit-test fixture for application handler tests.
/// </summary>
public abstract class TestBase : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestBase" /> class.
    /// </summary>
    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"ProjectManagementTests-{Guid.NewGuid()}")
            .EnableSensitiveDataLogging()
            .Options;

        DbContext = new ApplicationDbContext(options);

        var mapperConfiguration = new MapperConfiguration(
            configuration => configuration.AddProfile<MappingProfile>(),
            NullLoggerFactory.Instance);

        mapperConfiguration.AssertConfigurationIsValid();
        Mapper = mapperConfiguration.CreateMapper();

        SetCurrentUser(TestDataFactory.UserId, TestDataFactory.UserEmail);
        ConfigureJwtDefaults();
    }

    /// <summary>
    /// Gets the in-memory database context.
    /// </summary>
    protected ApplicationDbContext DbContext { get; }

    /// <summary>
    /// Gets the AutoMapper instance configured with application mappings.
    /// </summary>
    protected IMapper Mapper { get; }

    /// <summary>
    /// Gets the current-user service mock.
    /// </summary>
    protected Mock<ICurrentUserService> CurrentUserServiceMock { get; } = new();

    /// <summary>
    /// Gets the identity service mock.
    /// </summary>
    protected Mock<IIdentityService> IdentityServiceMock { get; } = new();

    /// <summary>
    /// Gets the JWT service mock.
    /// </summary>
    protected Mock<IJwtService> JwtServiceMock { get; } = new();

    /// <inheritdoc />
    public void Dispose()
    {
        DbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Sets the authenticated user exposed to handlers.
    /// </summary>
    /// <param name="userId">The current user identifier.</param>
    /// <param name="email">The current user email.</param>
    /// <param name="isAuthenticated">Whether the current request is authenticated.</param>
    protected void SetCurrentUser(
        string? userId,
        string? email = null,
        bool isAuthenticated = true)
    {
        CurrentUserServiceMock.SetupGet(service => service.UserId).Returns(userId);
        CurrentUserServiceMock.SetupGet(service => service.Email).Returns(email);
        CurrentUserServiceMock.SetupGet(service => service.IsAuthenticated).Returns(isAuthenticated);
    }

    /// <summary>
    /// Persists entities to the in-memory database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entities">The entities to persist.</param>
    /// <returns>A task that completes when entities are saved.</returns>
    protected async Task AddAsync<TEntity>(params TEntity[] entities)
        where TEntity : class
    {
        DbContext.Set<TEntity>().AddRange(entities);
        await DbContext.SaveChangesAsync(CancellationToken.None);
    }

    private void ConfigureJwtDefaults()
    {
        JwtServiceMock
            .Setup(service => service.GenerateAccessToken(
                It.IsAny<ApplicationUser>(),
                It.IsAny<IReadOnlyCollection<string>>()))
            .Returns("access-token");

        JwtServiceMock
            .Setup(service => service.GenerateRefreshToken())
            .Returns("refresh-token");

        JwtServiceMock
            .Setup(service => service.HashRefreshToken(It.IsAny<string>()))
            .Returns<string>(refreshToken => $"hashed-{refreshToken}");

        JwtServiceMock
            .Setup(service => service.GetAccessTokenExpiration(It.IsAny<DateTimeOffset>()))
            .Returns<DateTimeOffset>(utcNow => utcNow.AddMinutes(15));

        JwtServiceMock
            .Setup(service => service.GetRefreshTokenExpiration(It.IsAny<DateTimeOffset>()))
            .Returns<DateTimeOffset>(utcNow => utcNow.AddDays(7));
    }
}
