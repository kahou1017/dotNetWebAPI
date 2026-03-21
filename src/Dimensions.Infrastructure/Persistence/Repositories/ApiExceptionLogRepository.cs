using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Log;

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

    public async Task<PagedResult<ApiExceptionLogItemResponse>> GetListAsync(ApiExceptionLogListRequest request, CancellationToken cancellationToken = default)
    {
        var pageNo = request.PageNo <= 0 ? 1 : request.PageNo;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;
        var offset = (pageNo - 1) * pageSize;

        const string countSql = """
            SELECT COUNT(1)
            FROM ApiExceptionLogs
            WHERE (@CaseId IS NULL OR CaseId = @CaseId)
              AND (@Path IS NULL OR RequestPath = @Path)
              AND (@UserId IS NULL OR UserId = @UserId)
              AND (@TokenId IS NULL OR TokenId = @TokenId)
              AND (@DeviceId IS NULL OR DeviceId = @DeviceId)
              AND (@ClientIp IS NULL OR ClientIp = @ClientIp)
              AND (@StatusCode IS NULL OR StatusCode = @StatusCode)
              AND (@ErrorCode IS NULL OR ErrorCode = @ErrorCode)
              AND (@ExceptionType IS NULL OR ExceptionType = @ExceptionType)
              AND (@OccurredAtStart IS NULL OR OccurredAt >= @OccurredAtStart)
              AND (@OccurredAtEnd IS NULL OR OccurredAt <= @OccurredAtEnd);
            """;

        const string listSql = """
            SELECT
                CaseId,
                OccurredAt,
                HttpMethod,
                RequestPath,
                StatusCode,
                ExceptionType,
                ErrorCode,
                ErrorMessage,
                ClientIp,
                DeviceId,
                IsAuthenticated,
                TokenId,
                UserId,
                TokenType
            FROM ApiExceptionLogs
            WHERE (@CaseId IS NULL OR CaseId = @CaseId)
              AND (@Path IS NULL OR RequestPath = @Path)
              AND (@UserId IS NULL OR UserId = @UserId)
              AND (@TokenId IS NULL OR TokenId = @TokenId)
              AND (@DeviceId IS NULL OR DeviceId = @DeviceId)
              AND (@ClientIp IS NULL OR ClientIp = @ClientIp)
              AND (@StatusCode IS NULL OR StatusCode = @StatusCode)
              AND (@ErrorCode IS NULL OR ErrorCode = @ErrorCode)
              AND (@ExceptionType IS NULL OR ExceptionType = @ExceptionType)
              AND (@OccurredAtStart IS NULL OR OccurredAt >= @OccurredAtStart)
              AND (@OccurredAtEnd IS NULL OR OccurredAt <= @OccurredAtEnd)
            ORDER BY OccurredAt DESC, CaseId DESC
            LIMIT @PageSize OFFSET @Offset;
            """;

        var parameters = new
        {
            request.CaseId,
            request.Path,
            request.UserId,
            request.TokenId,
            request.DeviceId,
            request.ClientIp,
            request.StatusCode,
            request.ErrorCode,
            request.ExceptionType,
            request.OccurredAtStart,
            request.OccurredAtEnd,
            PageSize = pageSize,
            Offset = offset
        };

        var totalCount = await sqlExecutor.QuerySingleOrDefaultAsync<int>(countSql, parameters, cancellationToken: cancellationToken);
        var items = await sqlExecutor.QueryAsync<ApiExceptionLogItemResponse>(listSql, parameters, cancellationToken: cancellationToken);

        return new PagedResult<ApiExceptionLogItemResponse>
        {
            PageNo = pageNo,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }
}
