using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Token;

namespace Dimensions.Infrastructure.Persistence.Repositories;

public sealed class TokenRepository(DapperSqlExecutor sqlExecutor) : ITokenRepository
{
    public async Task<PagedResult<TokenListItemResponse>> GetTokenListAsync(TokenListRequest request, CancellationToken cancellationToken = default)
    {
        var pageNo = request.PageNo <= 0 ? 1 : request.PageNo;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;
        var offset = (pageNo - 1) * pageSize;

        const string countSql = """
            SELECT COUNT(1)
            FROM Tokens
            WHERE (@TokenType IS NULL OR TokenType = @TokenType)
              AND (@UserId IS NULL OR UserId = @UserId)
              AND (@TokenId IS NULL OR TokenId = @TokenId)
              AND (@Status IS NULL OR Status = @Status)
              AND (@IsSingleDevice IS NULL OR IsSingleDevice = @IsSingleDevice)
              AND (@DeviceId IS NULL OR DeviceId = @DeviceId)
              AND (@IssuedAtStart IS NULL OR IssuedAt >= @IssuedAtStart)
              AND (@IssuedAtEnd IS NULL OR IssuedAt <= @IssuedAtEnd)
              AND (@ExpireAtStart IS NULL OR ExpireAt >= @ExpireAtStart)
              AND (@ExpireAtEnd IS NULL OR ExpireAt <= @ExpireAtEnd);
            """;

        const string listSql = """
            SELECT
                TokenId,
                TokenType,
                UserId,
                UserName,
                TokenName,
                Status,
                IsSingleDevice,
                DeviceId,
                IssuedAt,
                EffectiveAt,
                ExpireAt,
                LastUsedAt,
                CreatedBy
            FROM Tokens
            WHERE (@TokenType IS NULL OR TokenType = @TokenType)
              AND (@UserId IS NULL OR UserId = @UserId)
              AND (@TokenId IS NULL OR TokenId = @TokenId)
              AND (@Status IS NULL OR Status = @Status)
              AND (@IsSingleDevice IS NULL OR IsSingleDevice = @IsSingleDevice)
              AND (@DeviceId IS NULL OR DeviceId = @DeviceId)
              AND (@IssuedAtStart IS NULL OR IssuedAt >= @IssuedAtStart)
              AND (@IssuedAtEnd IS NULL OR IssuedAt <= @IssuedAtEnd)
              AND (@ExpireAtStart IS NULL OR ExpireAt >= @ExpireAtStart)
              AND (@ExpireAtEnd IS NULL OR ExpireAt <= @ExpireAtEnd)
            ORDER BY CreatedAt DESC, TokenId DESC
            LIMIT @PageSize OFFSET @Offset;
            """;

        var parameters = new
        {
            request.TokenType,
            request.UserId,
            request.TokenId,
            request.Status,
            request.IsSingleDevice,
            request.DeviceId,
            request.IssuedAtStart,
            request.IssuedAtEnd,
            request.ExpireAtStart,
            request.ExpireAtEnd,
            PageSize = pageSize,
            Offset = offset
        };

        var totalCount = await sqlExecutor.QuerySingleOrDefaultAsync<int>(countSql, parameters, cancellationToken: cancellationToken);
        var items = await sqlExecutor.QueryAsync<TokenListItemResponse>(listSql, parameters, cancellationToken: cancellationToken);

        return new PagedResult<TokenListItemResponse>
        {
            PageNo = pageNo,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }

    public Task<TokenDetailResponse?> GetTokenDetailAsync(string tokenId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                TokenId,
                JwtId,
                TokenType,
                TokenName,
                UserId,
                UserName,
                Status,
                IsRevoked,
                IsSingleDevice,
                IsEnabled,
                CanReissue,
                CanRenew,
                DeviceId,
                DeviceName,
                IssuedAt,
                EffectiveAt,
                ExpireAt,
                LastUsedAt,
                Purpose,
                Remark,
                CreatedBy,
                CreatedAt
            FROM Tokens
            WHERE TokenId = @TokenId
            LIMIT 1;
            """;

        return sqlExecutor.QuerySingleOrDefaultAsync<TokenDetailResponse>(
            sql,
            new { TokenId = tokenId },
            cancellationToken: cancellationToken);
    }

    public Task<TokenDetailResponse?> GetValidTokenAsync(
        string tokenId,
        string jwtId,
        string tokenType,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                TokenId,
                JwtId,
                TokenType,
                TokenName,
                UserId,
                UserName,
                Status,
                IsRevoked,
                IsSingleDevice,
                IsEnabled,
                CanReissue,
                CanRenew,
                DeviceId,
                DeviceName,
                IssuedAt,
                EffectiveAt,
                ExpireAt,
                LastUsedAt,
                Purpose,
                Remark,
                CreatedBy,
                CreatedAt
            FROM Tokens
            WHERE TokenId = @TokenId
              AND JwtId = @JwtId
              AND TokenType = @TokenType
              AND Status = 'Active'
              AND IsEnabled = 1
              AND IsRevoked = 0
              AND EffectiveAt <= @Now
              AND (ExpireAt IS NULL OR ExpireAt >= @Now)
            LIMIT 1;
            """;

        return sqlExecutor.QuerySingleOrDefaultAsync<TokenDetailResponse>(
            sql,
            new
            {
                TokenId = tokenId,
                JwtId = jwtId,
                TokenType = tokenType,
                Now = DateTimeOffset.UtcNow
            },
            cancellationToken: cancellationToken);
    }

    public async Task InsertTokenAsync(TokenWriteModel token, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Tokens (
                TokenId, JwtId, TokenType, UserId, UserName, TokenName, Status, IsRevoked, IsSingleDevice, IsEnabled,
                CanReissue, CanRenew, DeviceId, DeviceName, AccessToken, IssuedAt, EffectiveAt, ExpireAt, LastUsedAt,
                Purpose, Remark, CreatedBy, CreatedAt, RevokedAt)
            VALUES (
                @TokenId, @JwtId, @TokenType, @UserId, @UserName, @TokenName, @Status, @IsRevoked, @IsSingleDevice, @IsEnabled,
                @CanReissue, @CanRenew, @DeviceId, @DeviceName, @AccessToken, @IssuedAt, @EffectiveAt, @ExpireAt, @LastUsedAt,
                @Purpose, @Remark, @CreatedBy, @CreatedAt, @RevokedAt);
            """;

        await sqlExecutor.ExecuteAsync(sql, token, cancellationToken: cancellationToken);
    }

    public async Task UpdateTokenStatusAsync(
        string tokenId,
        string status,
        bool isRevoked,
        DateTimeOffset? revokedAt,
        DateTimeOffset? expireAt,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Tokens
            SET Status = @Status,
                IsRevoked = @IsRevoked,
                RevokedAt = COALESCE(@RevokedAt, RevokedAt),
                ExpireAt = COALESCE(@ExpireAt, ExpireAt)
            WHERE TokenId = @TokenId;
            """;

        await sqlExecutor.ExecuteAsync(
            sql,
            new
            {
                TokenId = tokenId,
                Status = status,
                IsRevoked = isRevoked,
                RevokedAt = revokedAt,
                ExpireAt = expireAt
            },
            cancellationToken: cancellationToken);
    }

    public async Task<PagedResult<TokenUsageItemResponse>> GetTokenUsageAsync(TokenUsageRequest request, CancellationToken cancellationToken = default)
    {
        var pageNo = request.PageNo <= 0 ? 1 : request.PageNo;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;
        var offset = (pageNo - 1) * pageSize;

        const string countSql = """
            SELECT COUNT(1)
            FROM TokenUsageLogs
            WHERE (@TokenId IS NULL OR TokenId = @TokenId)
              AND (@UserId IS NULL OR UserId = @UserId)
              AND (@DeviceId IS NULL OR DeviceId = @DeviceId)
              AND (@ClientIp IS NULL OR ClientIp = @ClientIp)
              AND (@IsSuccess IS NULL OR IsSuccess = @IsSuccess)
              AND (@RequestTimeStart IS NULL OR RequestTime >= @RequestTimeStart)
              AND (@RequestTimeEnd IS NULL OR RequestTime <= @RequestTimeEnd);
            """;

        const string listSql = """
            SELECT
                CaseId,
                TokenId,
                UserId,
                RequestTime,
                HttpMethod,
                RequestPath,
                ClientIp,
                DeviceId,
                IsSuccess,
                FailureReason
            FROM TokenUsageLogs
            WHERE (@TokenId IS NULL OR TokenId = @TokenId)
              AND (@UserId IS NULL OR UserId = @UserId)
              AND (@DeviceId IS NULL OR DeviceId = @DeviceId)
              AND (@ClientIp IS NULL OR ClientIp = @ClientIp)
              AND (@IsSuccess IS NULL OR IsSuccess = @IsSuccess)
              AND (@RequestTimeStart IS NULL OR RequestTime >= @RequestTimeStart)
              AND (@RequestTimeEnd IS NULL OR RequestTime <= @RequestTimeEnd)
            ORDER BY RequestTime DESC
            LIMIT @PageSize OFFSET @Offset;
            """;

        var parameters = new
        {
            request.TokenId,
            request.UserId,
            request.DeviceId,
            request.ClientIp,
            request.IsSuccess,
            request.RequestTimeStart,
            request.RequestTimeEnd,
            PageSize = pageSize,
            Offset = offset
        };

        var totalCount = await sqlExecutor.QuerySingleOrDefaultAsync<int>(countSql, parameters, cancellationToken: cancellationToken);
        var items = await sqlExecutor.QueryAsync<TokenUsageItemResponse>(listSql, parameters, cancellationToken: cancellationToken);

        return new PagedResult<TokenUsageItemResponse>
        {
            PageNo = pageNo,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task<PagedResult<TokenActionLogItemResponse>> GetTokenActionLogAsync(TokenActionLogRequest request, CancellationToken cancellationToken = default)
    {
        var pageNo = request.PageNo <= 0 ? 1 : request.PageNo;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;
        var offset = (pageNo - 1) * pageSize;

        const string countSql = """
            SELECT COUNT(1)
            FROM TokenActionLogs
            WHERE (@TokenId IS NULL OR TokenId = @TokenId)
              AND (@ActionType IS NULL OR ActionType = @ActionType)
              AND (@OperatorUserId IS NULL OR OperatorUserId = @OperatorUserId)
              AND (@CreatedAtStart IS NULL OR CreatedAt >= @CreatedAtStart)
              AND (@CreatedAtEnd IS NULL OR CreatedAt <= @CreatedAtEnd);
            """;

        const string listSql = """
            SELECT
                CaseId,
                TokenId,
                ActionType,
                ActionReason,
                ActionResult,
                OperatorUserId,
                OperatorUserName,
                BeforeStatus,
                AfterStatus,
                CreatedAt
            FROM TokenActionLogs
            WHERE (@TokenId IS NULL OR TokenId = @TokenId)
              AND (@ActionType IS NULL OR ActionType = @ActionType)
              AND (@OperatorUserId IS NULL OR OperatorUserId = @OperatorUserId)
              AND (@CreatedAtStart IS NULL OR CreatedAt >= @CreatedAtStart)
              AND (@CreatedAtEnd IS NULL OR CreatedAt <= @CreatedAtEnd)
            ORDER BY CreatedAt DESC
            LIMIT @PageSize OFFSET @Offset;
            """;

        var parameters = new
        {
            request.TokenId,
            request.ActionType,
            request.OperatorUserId,
            request.CreatedAtStart,
            request.CreatedAtEnd,
            PageSize = pageSize,
            Offset = offset
        };

        var totalCount = await sqlExecutor.QuerySingleOrDefaultAsync<int>(countSql, parameters, cancellationToken: cancellationToken);
        var items = await sqlExecutor.QueryAsync<TokenActionLogItemResponse>(listSql, parameters, cancellationToken: cancellationToken);

        return new PagedResult<TokenActionLogItemResponse>
        {
            PageNo = pageNo,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task InsertActionLogAsync(TokenActionLogItemResponse item, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO TokenActionLogs (
                CaseId, TokenId, ActionType, ActionReason, ActionResult, OperatorUserId, OperatorUserName, BeforeStatus, AfterStatus, CreatedAt)
            VALUES (
                @CaseId, @TokenId, @ActionType, @ActionReason, @ActionResult, @OperatorUserId, @OperatorUserName, @BeforeStatus, @AfterStatus, @CreatedAt);
            """;

        await sqlExecutor.ExecuteAsync(sql, item, cancellationToken: cancellationToken);
    }

    public async Task UpdateLastUsedAtAsync(string tokenId, DateTimeOffset lastUsedAt, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Tokens
            SET LastUsedAt = @LastUsedAt
            WHERE TokenId = @TokenId;
            """;

        await sqlExecutor.ExecuteAsync(
            sql,
            new { TokenId = tokenId, LastUsedAt = lastUsedAt },
            cancellationToken: cancellationToken);
    }

    public async Task InsertUsageLogAsync(TokenUsageItemResponse item, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO TokenUsageLogs (
                CaseId, TokenId, UserId, RequestTime, HttpMethod, RequestPath, ClientIp, DeviceId, IsSuccess, FailureReason)
            VALUES (
                @CaseId, @TokenId, @UserId, @RequestTime, @HttpMethod, @RequestPath, @ClientIp, @DeviceId, @IsSuccess, @FailureReason);
            """;

        await sqlExecutor.ExecuteAsync(sql, item, cancellationToken: cancellationToken);
    }
}
