using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Log;

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

    public async Task<PagedResult<ApiRequestLogItemResponse>> GetListAsync(ApiRequestLogListRequest request, CancellationToken cancellationToken = default)
    {
        var pageNo = request.PageNo <= 0 ? 1 : request.PageNo;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;
        var offset = (pageNo - 1) * pageSize;

        const string countSql = """
            SELECT COUNT(1)
            FROM ApiRequestLogs
            WHERE (@CaseId IS NULL OR CaseId = @CaseId)
              AND (@Path IS NULL OR RequestPath = @Path)
              AND (@UserId IS NULL OR UserId = @UserId)
              AND (@TokenId IS NULL OR TokenId = @TokenId)
              AND (@DeviceId IS NULL OR DeviceId = @DeviceId)
              AND (@ClientIp IS NULL OR ClientIp = @ClientIp)
              AND (@StatusCode IS NULL OR StatusCode = @StatusCode)
              AND (@IsAuthenticated IS NULL OR IsAuthenticated = @IsAuthenticated)
              AND (@IsSuccess IS NULL OR IsSuccess = @IsSuccess)
              AND (@RequestTimeStart IS NULL OR RequestTime >= @RequestTimeStart)
              AND (@RequestTimeEnd IS NULL OR RequestTime <= @RequestTimeEnd);
            """;

        const string listSql = """
            SELECT
                CaseId,
                RequestTime,
                HttpMethod,
                RequestPath,
                StatusCode,
                ClientIp,
                DeviceId,
                IsAuthenticated,
                IsSuccess,
                TokenId,
                UserId,
                TokenType
            FROM ApiRequestLogs
            WHERE (@CaseId IS NULL OR CaseId = @CaseId)
              AND (@Path IS NULL OR RequestPath = @Path)
              AND (@UserId IS NULL OR UserId = @UserId)
              AND (@TokenId IS NULL OR TokenId = @TokenId)
              AND (@DeviceId IS NULL OR DeviceId = @DeviceId)
              AND (@ClientIp IS NULL OR ClientIp = @ClientIp)
              AND (@StatusCode IS NULL OR StatusCode = @StatusCode)
              AND (@IsAuthenticated IS NULL OR IsAuthenticated = @IsAuthenticated)
              AND (@IsSuccess IS NULL OR IsSuccess = @IsSuccess)
              AND (@RequestTimeStart IS NULL OR RequestTime >= @RequestTimeStart)
              AND (@RequestTimeEnd IS NULL OR RequestTime <= @RequestTimeEnd)
            ORDER BY RequestTime DESC, CaseId DESC
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
            request.IsAuthenticated,
            request.IsSuccess,
            request.RequestTimeStart,
            request.RequestTimeEnd,
            PageSize = pageSize,
            Offset = offset
        };

        var totalCount = await sqlExecutor.QuerySingleOrDefaultAsync<int>(countSql, parameters, cancellationToken: cancellationToken);
        var items = await sqlExecutor.QueryAsync<ApiRequestLogItemResponse>(listSql, parameters, cancellationToken: cancellationToken);

        return new PagedResult<ApiRequestLogItemResponse>
        {
            PageNo = pageNo,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }
}
