using Dimensions.Application.Models;

namespace Dimensions.Application.Interfaces;

public interface IAdminRepository
{
    Task<AdminUserData?> GetByLoginAccountAsync(string loginAccount, CancellationToken cancellationToken = default);

    Task<AdminUserData?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    Task UpdateLastLoginAtAsync(string userId, DateTimeOffset lastLoginAt, CancellationToken cancellationToken = default);
}
