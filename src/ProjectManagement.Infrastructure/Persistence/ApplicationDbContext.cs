using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for the application.
/// </summary>
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
{
    /// <inheritdoc />
    public DbSet<Project> Projects => Set<Project>();

    /// <inheritdoc />
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();

    /// <inheritdoc />
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        ApplySoftDeleteQueryFilters(builder);
    }

    private static void ApplySoftDeleteQueryFilters(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var parameter = Expression.Parameter(entityType.ClrType, "entity");
            var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
            var comparison = Expression.Equal(property, Expression.Constant(false));
            var lambda = Expression.Lambda(comparison, parameter);

            builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }
}
