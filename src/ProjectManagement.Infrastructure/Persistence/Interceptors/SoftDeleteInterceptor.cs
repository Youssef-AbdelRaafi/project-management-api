using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ProjectManagement.Domain.Common;

namespace ProjectManagement.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Converts EF Core delete operations into soft-delete updates.
/// </summary>
public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{
    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        SoftDeleteEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SoftDeleteEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SoftDeleteEntities(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var utcNow = DateTimeOffset.UtcNow;
        var entries = context.ChangeTracker
            .Entries<ISoftDelete>()
            .Where(entry => entry.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            entry.State = EntityState.Modified;
            entry.Entity.MarkAsDeleted(utcNow);
        }
    }
}
