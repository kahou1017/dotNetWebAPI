using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;

namespace Dimensions.Infrastructure.Persistence.Repositories;

public sealed class AdminRepository(DapperSqlExecutor sqlExecutor) : IAdminRepository
{
    public Task<AdminUserData?> GetByLoginAccountAsync(string loginAccount, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                UserId,
                LoginAccount,
                Password,
                DisplayName,
                LastLoginAt
            FROM AdminUsers
            WHERE LoginAccount = @LoginAccount
            LIMIT 1;
            """;

        return sqlExecutor.QuerySingleOrDefaultAsync<AdminUserData>(
            sql,
            new { LoginAccount = loginAccount },
            cancellationToken: cancellationToken);
    }

    public Task<AdminUserData?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                UserId,
                LoginAccount,
                Password,
                DisplayName,
                LastLoginAt
            FROM AdminUsers
            WHERE UserId = @UserId
            LIMIT 1;
            """;

        return sqlExecutor.QuerySingleOrDefaultAsync<AdminUserData>(
            sql,
            new { UserId = userId },
            cancellationToken: cancellationToken);
    }

    public async Task UpdateLastLoginAtAsync(string userId, DateTimeOffset lastLoginAt, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE AdminUsers
            SET LastLoginAt = @LastLoginAt
            WHERE UserId = @UserId;
            """;

        await sqlExecutor.ExecuteAsync(
            sql,
            new { UserId = userId, LastLoginAt = lastLoginAt },
            cancellationToken: cancellationToken);
    }
}
