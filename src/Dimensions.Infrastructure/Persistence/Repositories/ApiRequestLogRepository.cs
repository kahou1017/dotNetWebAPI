using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;

namespace Dimensions.Infrastructure.Persistence.Repositories;

public sealed class ApiRequestLogRepository(DapperSqlExecutor sqlExecutor) : IApiRequestLogRepository
{
    public Task InsertAsync(ApiRequestLogRecord record, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO ApiRequestLogs (
                CaseId, RequestTime, HttpMethod, RequestPath, StatusCode, ClientIp, DeviceId, IsAuthenticated, IsSuccess, TokenId, UserId, TokenType)
            VALUES (
                @CaseId, @RequestTime, @Method, @Path, @StatusCode, @ClientIp, @DeviceId, @IsAuthenticated, @IsSuccess, @TokenId, @UserId, @TokenType);
            """;

        return sqlExecutor.ExecuteAsync(sql, record, cancellationToken: cancellationToken);
    }
}
