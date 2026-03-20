using Dimensions.Application.Exceptions;
using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Token;
using Dimensions.Domain.Enums;

namespace Dimensions.Application.Services;

public sealed class TokenService(
    ITokenRepository tokenRepository,
    IDeviceRepository deviceRepository,
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
        await ValidateSingleDeviceBindingAsync(
            request.UserId,
            request.IsSingleDevice,
            request.DeviceId,
            "Token.DeviceBindingInvalid",
            cancellationToken);

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
        var detail = await GetRequiredTokenAsync(request.TokenId, cancellationToken);
        var currentStatus = await NormalizeStatusIfExpiredAsync(detail, cancellationToken);

        EnsureManageableToken(detail, currentStatus, "revoke");
        EnsureNotTerminalStatus(currentStatus, allowExpired: true, "revoke");

        await tokenRepository.UpdateTokenStatusAsync(request.TokenId, TokenStatus.Revoked, true, now, null, cancellationToken);
        await tokenRepository.InsertActionLogAsync(
            BuildActionLog(
                request.TokenId,
                "RevokeToken",
                request.Reason,
                beforeStatus: currentStatus,
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
        var oldDetail = await GetRequiredTokenAsync(request.TokenId, cancellationToken);
        var currentStatus = await NormalizeStatusIfExpiredAsync(oldDetail, cancellationToken);
        EnsureManageableToken(oldDetail, currentStatus, "reissue");
        EnsureNotTerminalStatus(currentStatus, allowExpired: true, "reissue");

        if (!oldDetail.CanReissue)
        {
            throw CreateException(409, "Token.ReissueNotAllowed", "This token cannot be reissued.");
        }

        var newDeviceId = request.DeviceId ?? oldDetail.DeviceId;
        if (oldDetail.IsSingleDevice)
        {
            await ValidateSingleDeviceBindingAsync(
                oldDetail.UserId,
                true,
                newDeviceId,
                "Token.DeviceBindingInvalid",
                cancellationToken);
        }

        var newTokenId = $"TK{now:yyyyMMddHHmmss}";
        var newJwtId = Guid.NewGuid().ToString("N");
        var tokenType = oldDetail.TokenType;
        var role = tokenType == TokenType.Integration ? "Integration" : "User";
        var scope = tokenType == TokenType.Integration ? "customer.query account.update" : "customer.query order.create";
        var token = jwtTokenGenerator.GenerateToken(new JwtTokenRequest
        {
            UserId = oldDetail.UserId,
            DisplayName = oldDetail.UserName,
            Role = role,
            Scope = scope,
            TokenType = tokenType,
            TokenId = newTokenId,
            JwtId = newJwtId,
            IssuedAt = now,
            EffectiveAt = request.EffectiveAt,
            ExpiresAt = request.ExpireAt ?? oldDetail.ExpireAt
        });

        await tokenRepository.UpdateTokenStatusAsync(request.TokenId, TokenStatus.Reissued, true, now, null, cancellationToken);
        await tokenRepository.InsertTokenAsync(
            new TokenWriteModel
            {
                TokenId = newTokenId,
                JwtId = newJwtId,
                TokenType = tokenType,
                UserId = oldDetail.UserId,
                UserName = oldDetail.UserName,
                TokenName = oldDetail.TokenName,
                Status = TokenStatus.Active,
                IsRevoked = false,
                IsSingleDevice = oldDetail.IsSingleDevice,
                IsEnabled = oldDetail.IsEnabled,
                CanReissue = oldDetail.CanReissue,
                CanRenew = oldDetail.CanRenew,
                DeviceId = newDeviceId,
                DeviceName = request.DeviceName ?? oldDetail.DeviceName,
                AccessToken = token.AccessToken,
                IssuedAt = token.IssuedAt,
                EffectiveAt = token.EffectiveAt,
                ExpireAt = token.ExpiresAt,
                LastUsedAt = null,
                Purpose = oldDetail.Purpose,
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
                beforeStatus: currentStatus,
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
        var detail = await GetRequiredTokenAsync(request.TokenId, cancellationToken);
        var currentStatus = await NormalizeStatusIfExpiredAsync(detail, cancellationToken);
        EnsureManageableToken(detail, currentStatus, "renew");
        EnsureNotTerminalStatus(currentStatus, allowExpired: true, "renew");

        if (!detail.CanRenew)
        {
            throw CreateException(409, "Token.RenewNotAllowed", "This token cannot be renewed.");
        }

        var now = DateTimeOffset.UtcNow;
        var newExpireAt = request.NewExpireAt ?? detail.ExpireAt;
        if (newExpireAt is not null && newExpireAt <= now)
        {
            throw CreateException(400, "Token.ExpireAtInvalid", "The renewed expiration time must be later than now.");
        }

        var newJwtId = Guid.NewGuid().ToString("N");
        var role = detail.TokenType == TokenType.Integration ? "Integration" : "User";
        var scope = detail.TokenType == TokenType.Integration ? "customer.query account.update" : "customer.query order.create";
        var token = jwtTokenGenerator.GenerateToken(new JwtTokenRequest
        {
            UserId = detail.UserId,
            DisplayName = detail.UserName,
            Role = role,
            Scope = scope,
            TokenType = detail.TokenType,
            TokenId = detail.TokenId,
            JwtId = newJwtId,
            IssuedAt = now,
            EffectiveAt = now,
            ExpiresAt = newExpireAt
        });

        await tokenRepository.UpdateTokenCredentialsAsync(
            request.TokenId,
            newJwtId,
            token.AccessToken,
            token.IssuedAt,
            token.EffectiveAt,
            token.ExpiresAt,
            cancellationToken);

        await tokenRepository.InsertActionLogAsync(
            BuildActionLog(
                request.TokenId,
                "RenewToken",
                request.Reason,
                beforeStatus: currentStatus,
                afterStatus: TokenStatus.Active),
            cancellationToken);

        return new RenewTokenResponse
        {
            TokenId = request.TokenId,
            JwtId = newJwtId,
            AccessToken = token.AccessToken,
            OldExpireAt = detail.ExpireAt,
            NewExpireAt = token.ExpiresAt,
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

    private async Task ValidateSingleDeviceBindingAsync(
        string userId,
        bool isSingleDevice,
        string? deviceId,
        string errorCode,
        CancellationToken cancellationToken)
    {
        if (!isSingleDevice)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(deviceId))
        {
            return;
        }

        var device = await deviceRepository.GetDeviceByIdAsync(deviceId, cancellationToken);
        if (device is null
            || !device.IsEnabled
            || !string.Equals(device.UserId, userId, StringComparison.Ordinal))
        {
            throw CreateException(400, errorCode, "The specified device is not enabled for the target user.");
        }
    }

    private async Task<TokenDetailResponse> GetRequiredTokenAsync(string tokenId, CancellationToken cancellationToken)
    {
        var detail = await tokenRepository.GetTokenDetailAsync(tokenId, cancellationToken);
        if (detail is null)
        {
            throw CreateException(404, "Token.NotFound", "The specified token does not exist.");
        }

        return detail;
    }

    private async Task<string> NormalizeStatusIfExpiredAsync(TokenDetailResponse detail, CancellationToken cancellationToken)
    {
        if (detail.Status == TokenStatus.Active
            && detail.ExpireAt is not null
            && detail.ExpireAt < DateTimeOffset.UtcNow)
        {
            await tokenRepository.UpdateTokenStatusAsync(
                detail.TokenId,
                TokenStatus.Expired,
                detail.IsRevoked,
                revokedAt: null,
                expireAt: detail.ExpireAt,
                cancellationToken);

            return TokenStatus.Expired;
        }

        return detail.Status;
    }

    private void EnsureManageableToken(TokenDetailResponse detail, string effectiveStatus, string actionName)
    {
        if (detail.TokenType == TokenType.AdminSession)
        {
            throw CreateException(
                400,
                "Token.AdminSessionNotAllowed",
                $"Admin session token cannot be used for {actionName}.");
        }

        if (!detail.IsEnabled || effectiveStatus == TokenStatus.Disabled)
        {
            throw CreateException(409, "Token.Disabled", "The token has been disabled.");
        }

        if (detail.IsRevoked || effectiveStatus == TokenStatus.Revoked)
        {
            throw CreateException(409, "Token.Revoked", "The token has already been revoked.");
        }
    }

    private void EnsureNotTerminalStatus(string effectiveStatus, bool allowExpired, string actionName)
    {
        if (effectiveStatus == TokenStatus.Reissued)
        {
            throw CreateException(409, "Token.Reissued", $"The token has already been reissued and cannot be {actionName}d again.");
        }

        if (!allowExpired && effectiveStatus == TokenStatus.Expired)
        {
            throw CreateException(409, "Token.Expired", $"The token has already expired and cannot be {actionName}d.");
        }
    }

    private static DimensionsApplicationException CreateException(int statusCode, string errorCode, string errorMessage)
        => new(statusCode, errorCode, errorMessage);
}
