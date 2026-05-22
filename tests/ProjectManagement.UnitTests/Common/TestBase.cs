using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Mappings;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Persistence;

namespace ProjectManagement.UnitTests.Common;

public abstract class TestBase : IDisposable
{
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

    protected ApplicationDbContext DbContext { get; }

    protected IMapper Mapper { get; }

    protected Mock<ICurrentUserService> CurrentUserServiceMock { get; } = new();

    protected Mock<IIdentityService> IdentityServiceMock { get; } = new();

    protected Mock<IJwtService> JwtServiceMock { get; } = new();

    public void Dispose()
    {
        DbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    protected void SetCurrentUser(
        string? userId,
        string? email = null,
        bool isAuthenticated = true)
    {
        CurrentUserServiceMock.SetupGet(service => service.UserId).Returns(userId);
        CurrentUserServiceMock.SetupGet(service => service.Email).Returns(email);
        CurrentUserServiceMock.SetupGet(service => service.IsAuthenticated).Returns(isAuthenticated);
    }

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
