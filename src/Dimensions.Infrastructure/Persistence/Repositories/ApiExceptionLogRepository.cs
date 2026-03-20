using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;

namespace Dimensions.Infrastructure.Persistence.Repositories;

public sealed class ApiExceptionLogRepository(DapperSqlExecutor sqlExecutor) : IApiExceptionLogRepository
{
    public Task InsertAsync(ApiExceptionLogRecord record, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO ApiExceptionLogs (
                CaseId, OccurredAt, HttpMethod, RequestPath, StatusCode, ExceptionType, ErrorCode, ErrorMessage, ClientIp, DeviceId, IsAuthenticated, TokenId, UserId, TokenType)
            VALUES (
                @CaseId, @OccurredAt, @Method, @Path, @StatusCode, @ExceptionType, @ErrorCode, @Message, @ClientIp, @DeviceId, @IsAuthenticated, @TokenId, @UserId, @TokenType);
            """;

        return sqlExecutor.ExecuteAsync(sql, record, cancellationToken: cancellationToken);
    }
}
