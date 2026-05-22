using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Project> Projects { get; }

    DbSet<TaskItem> TaskItems { get; }

    DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
