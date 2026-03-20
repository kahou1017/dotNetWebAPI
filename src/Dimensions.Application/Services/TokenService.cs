using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Token;
using Dimensions.Domain.Enums;

namespace Dimensions.Application.Services;

public sealed class TokenService(
    ITokenRepository tokenRepository,
    ICaseIdAccessor caseIdAccessor,
    ICurrentUserAccessor currentUserAccessor,
    IJwtTokenGenerator jwtTokenGenerator) : ITokenService
{
    public Task<PagedResult<TokenListItemResponse>> GetTokenListAsync(TokenListRequest request, CancellationToken cancellationToken = default)
        => tokenRepository.GetTokenListAsync(request, cancellationToken);

    public async Task<TokenDetailResponse> GetTokenDetailAsync(TokenDetailRequest request, CancellationToken cancellationToken = default)
    {
        var detail = await tokenRepository.GetTokenDetailAsync(request.TokenId, cancellationToken);

        return detail ?? new TokenDetailResponse
        {
            TokenId = request.TokenId
        };
    }

    public async Task<CreateTokenResponse> CreateTokenAsync(CreateTokenRequest request, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var tokenId = $"TK{now:yyyyMMddHHmmss}";
        var jwtId = Guid.NewGuid().ToString("N");
        var expiresAt = request.IsPermanent ? null : request.ExpireAt;
        var role = request.TokenType == TokenType.Integration ? "Integration" : "User";
        var scope = request.TokenType == TokenType.Integration ? "customer.query account.update" : "customer.query order.create";
        var token = jwtTokenGenerator.GenerateToken(new JwtTokenRequest
        {
            UserId = request.UserId,
            DisplayName = request.UserName,
            Role = role,
            Scope = scope,
            TokenType = request.TokenType,
            TokenId = tokenId,
            JwtId = jwtId,
            IssuedAt = now,
            EffectiveAt = request.EffectiveAt,
            ExpiresAt = expiresAt
        });

        await tokenRepository.InsertTokenAsync(
            new TokenWriteModel
            {
                TokenId = tokenId,
                JwtId = jwtId,
                TokenType = request.TokenType,
                UserId = request.UserId,
                UserName = request.UserName,
                TokenName = request.TokenName,
                Status = TokenStatus.Active,
                IsRevoked = false,
                IsSingleDevice = request.IsSingleDevice,
                IsEnabled = true,
                CanReissue = request.CanReissue,
                CanRenew = request.CanRenew,
                DeviceId = request.DeviceId,
                DeviceName = request.DeviceName,
                AccessToken = token.AccessToken,
                IssuedAt = token.IssuedAt,
                EffectiveAt = token.EffectiveAt,
                ExpireAt = token.ExpiresAt,
                LastUsedAt = null,
                Purpose = request.Purpose,
                Remark = request.Remark,
                CreatedBy = currentUserAccessor.GetUserId() ?? "ADMIN001",
                CreatedAt = now,
                RevokedAt = null
            },
            cancellationToken);

        await tokenRepository.InsertActionLogAsync(
            BuildActionLog(
                tokenId,
                "CreateToken",
                request.Remark ?? request.Purpose ?? "create token",
                beforeStatus: null,
                afterStatus: TokenStatus.Active),
            cancellationToken);

        return new CreateTokenResponse
        {
            TokenId = tokenId,
            JwtId = jwtId,
            TokenType = request.TokenType,
            AccessToken = token.AccessToken,
            IssuedAt = token.IssuedAt,
            EffectiveAt = token.EffectiveAt,
            ExpireAt = token.ExpiresAt,
            Status = TokenStatus.Active
        };
    }

    public async Task<RevokeTokenResponse> RevokeTokenAsync(RevokeTokenRequest request, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var detail = await tokenRepository.GetTokenDetailAsync(request.TokenId, cancellationToken);

        await tokenRepository.UpdateTokenStatusAsync(request.TokenId, TokenStatus.Revoked, true, now, null, cancellationToken);
        await tokenRepository.InsertActionLogAsync(
            BuildActionLog(
                request.TokenId,
                "RevokeToken",
                request.Reason,
                beforeStatus: detail?.Status,
                afterStatus: TokenStatus.Revoked),
            cancellationToken);

        return new RevokeTokenResponse
        {
            TokenId = request.TokenId,
            Status = TokenStatus.Revoked,
            RevokedAt = now
        };
    }

    public async Task<ReissueTokenResponse> ReissueTokenAsync(ReissueTokenRequest request, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var oldDetail = await tokenRepository.GetTokenDetailAsync(request.TokenId, cancellationToken);
        var newTokenId = $"TK{now:yyyyMMddHHmmss}";
        var newJwtId = Guid.NewGuid().ToString("N");
        var tokenType = oldDetail?.TokenType ?? TokenType.UserAccess;
        var role = tokenType == TokenType.Integration ? "Integration" : "User";
        var scope = tokenType == TokenType.Integration ? "customer.query account.update" : "customer.query order.create";
        var token = jwtTokenGenerator.GenerateToken(new JwtTokenRequest
        {
            UserId = oldDetail?.UserId ?? "USER001",
            DisplayName = oldDetail?.UserName ?? "Kevin",
            Role = role,
            Scope = scope,
            TokenType = tokenType,
            TokenId = newTokenId,
            JwtId = newJwtId,
            IssuedAt = now,
            EffectiveAt = request.EffectiveAt,
            ExpiresAt = request.ExpireAt ?? oldDetail?.ExpireAt
        });

        await tokenRepository.UpdateTokenStatusAsync(request.TokenId, TokenStatus.Reissued, true, now, null, cancellationToken);
        await tokenRepository.InsertTokenAsync(
            new TokenWriteModel
            {
                TokenId = newTokenId,
                JwtId = newJwtId,
                TokenType = tokenType,
                UserId = oldDetail?.UserId ?? "USER001",
                UserName = oldDetail?.UserName ?? "Kevin",
                TokenName = oldDetail?.TokenName ?? "Reissued Token",
                Status = TokenStatus.Active,
                IsRevoked = false,
                IsSingleDevice = oldDetail?.IsSingleDevice ?? false,
                IsEnabled = oldDetail?.IsEnabled ?? true,
                CanReissue = oldDetail?.CanReissue ?? true,
                CanRenew = oldDetail?.CanRenew ?? true,
                DeviceId = request.DeviceId ?? oldDetail?.DeviceId,
                DeviceName = request.DeviceName ?? oldDetail?.DeviceName,
                AccessToken = token.AccessToken,
                IssuedAt = token.IssuedAt,
                EffectiveAt = token.EffectiveAt,
                ExpireAt = token.ExpiresAt,
                LastUsedAt = null,
                Purpose = oldDetail?.Purpose,
                Remark = request.Reason,
                CreatedBy = currentUserAccessor.GetUserId() ?? "ADMIN001",
                CreatedAt = now,
                RevokedAt = null
            },
            cancellationToken);

        await tokenRepository.InsertActionLogAsync(
            BuildActionLog(
                request.TokenId,
                "ReissueToken",
                request.Reason,
                beforeStatus: oldDetail?.Status,
                afterStatus: TokenStatus.Reissued),
            cancellationToken);

        return new ReissueTokenResponse
        {
            OldTokenId = request.TokenId,
            NewTokenId = newTokenId,
            NewJwtId = newJwtId,
            AccessToken = token.AccessToken,
            OldStatus = TokenStatus.Reissued,
            NewStatus = TokenStatus.Active
        };
    }

    public async Task<RenewTokenResponse> RenewTokenAsync(RenewTokenRequest request, CancellationToken cancellationToken = default)
    {
        var detail = await tokenRepository.GetTokenDetailAsync(request.TokenId, cancellationToken);

        await tokenRepository.UpdateTokenStatusAsync(
            request.TokenId,
            TokenStatus.Active,
            detail?.IsRevoked ?? false,
            revokedAt: null,
            expireAt: request.NewExpireAt,
            cancellationToken);

        await tokenRepository.InsertActionLogAsync(
            BuildActionLog(
                request.TokenId,
                "RenewToken",
                request.Reason,
                beforeStatus: detail?.Status,
                afterStatus: TokenStatus.Active),
            cancellationToken);

        return new RenewTokenResponse
        {
            TokenId = request.TokenId,
            OldExpireAt = detail?.ExpireAt,
            NewExpireAt = request.NewExpireAt,
            Status = TokenStatus.Active
        };
    }

    public Task<PagedResult<TokenUsageItemResponse>> GetTokenUsageAsync(TokenUsageRequest request, CancellationToken cancellationToken = default)
        => tokenRepository.GetTokenUsageAsync(request, cancellationToken);

    public Task<PagedResult<TokenActionLogItemResponse>> GetTokenActionLogAsync(TokenActionLogRequest request, CancellationToken cancellationToken = default)
        => tokenRepository.GetTokenActionLogAsync(request, cancellationToken);

    private TokenActionLogItemResponse BuildActionLog(
        string tokenId,
        string actionType,
        string? actionReason,
        string? beforeStatus,
        string? afterStatus)
    {
        return new TokenActionLogItemResponse
        {
            CaseId = caseIdAccessor.GetCaseId(),
            TokenId = tokenId,
            ActionType = actionType,
            ActionReason = actionReason,
            ActionResult = "Success",
            OperatorUserId = currentUserAccessor.GetUserId() ?? "ADMIN001",
            OperatorUserName = currentUserAccessor.GetDisplayName() ?? "System Admin",
            BeforeStatus = beforeStatus,
            AfterStatus = afterStatus,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
