using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Interfaces;

/// <summary>
/// Defines the persistence boundary required by application handlers.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Gets the project set.
    /// </summary>
    DbSet<Project> Projects { get; }

    /// <summary>
    /// Gets the task item set.
    /// </summary>
    DbSet<TaskItem> TaskItems { get; }

    /// <summary>
    /// Gets the refresh token set.
    /// </summary>
    DbSet<RefreshToken> RefreshTokens { get; }

    /// <summary>
    /// Saves pending persistence changes.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
