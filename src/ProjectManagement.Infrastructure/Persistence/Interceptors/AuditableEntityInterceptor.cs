using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Common;

namespace ProjectManagement.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Populates audit fields before EF Core persists changes.
/// </summary>
public sealed class AuditableEntityInterceptor(ICurrentUserService currentUserService) : SaveChangesInterceptor
{
    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var utcNow = DateTimeOffset.UtcNow;
        var userId = currentUserService.UserId;

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                SetCreated(entry, utcNow, userId);
            }

            if (entry.State == EntityState.Modified)
            {
                SetUpdated(entry, utcNow, userId);
            }
        }
    }

    private static void SetCreated(EntityEntry<IAuditableEntity> entry, DateTimeOffset utcNow, string? userId)
    {
        entry.Entity.CreatedAt = utcNow;
        entry.Entity.CreatedBy = userId;
    }

    private static void SetUpdated(EntityEntry<IAuditableEntity> entry, DateTimeOffset utcNow, string? userId)
    {
        entry.Entity.UpdatedAt = utcNow;
        entry.Entity.UpdatedBy = userId;

        entry.Property(entity => entity.CreatedAt).IsModified = false;
        entry.Property(entity => entity.CreatedBy).IsModified = false;
    }
}
