using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<ApplicationUser>> RegisterAsync(
        string email,
        string password,
        string fullName,
        CancellationToken cancellationToken);

    Task<Result<ApplicationUser>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken);

    Task<ApplicationUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<string>> GetUserRolesAsync(ApplicationUser user, CancellationToken cancellationToken);
}
