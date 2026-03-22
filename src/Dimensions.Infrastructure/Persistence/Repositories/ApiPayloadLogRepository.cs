using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;

namespace Dimensions.Infrastructure.Persistence.Repositories;

public sealed class ApiPayloadLogRepository(DapperSqlExecutor sqlExecutor) : IApiPayloadLogRepository
{
    public Task InsertAsync(ApiPayloadLogRecord record, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO ApiPayloadLogs (
                PayloadLogId, CaseId, CreatedAt, Direction, HttpMethod, RequestPath, ContentType, PayloadText, PayloadLength, IsTruncated, IsAuthenticated, TokenId, UserId, TokenType)
            VALUES (
                @PayloadLogId, @CaseId, @CreatedAt, @Direction, @Method, @Path, @ContentType, @PayloadText, @PayloadLength, @IsTruncated, @IsAuthenticated, @TokenId, @UserId, @TokenType);
            """;

        return sqlExecutor.ExecuteAsync(sql, record, cancellationToken: cancellationToken);
    }
}
