using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<UserAccount>> RegisterAsync(
        string email,
        string password,
        string fullName,
        CancellationToken cancellationToken);

    Task<Result<UserAccount>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken);

    Task<UserAccount?> GetUserByIdAsync(string userId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<string>> GetUserRolesAsync(UserAccount user, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<UserAccount>> GetAllUsersAsync(CancellationToken cancellationToken);
}
