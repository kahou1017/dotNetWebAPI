using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Contracts.Token;

namespace Dimensions.Application.Services;

// 目前只負責把已驗證成功的 token 請求寫入 usage log。
// 後續若要補 API request/exception/payload DB logging，應另外拆出專用 service。
public sealed class TokenUsageLogService(
    ICurrentUserAccessor currentUserAccessor,
    ITokenRepository tokenRepository) : ITokenUsageLogService
{
    public async Task LogRequestAsync(ApiRequestLogEntry entry, CancellationToken cancellationToken = default)
    {
        var tokenId = currentUserAccessor.GetTokenId();
        var userId = currentUserAccessor.GetUserId();
        var tokenType = currentUserAccessor.GetTokenType();

        if (string.IsNullOrWhiteSpace(tokenId)
            || string.IsNullOrWhiteSpace(userId)
            || string.IsNullOrWhiteSpace(tokenType))
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var item = new TokenUsageItemResponse
        {
            CaseId = entry.CaseId,
            TokenId = tokenId,
            UserId = userId,
            RequestTime = now,
            HttpMethod = entry.Method,
            RequestPath = entry.Path,
            ClientIp = entry.ClientIp,
            DeviceId = entry.DeviceId,
            IsSuccess = entry.StatusCode is >= 200 and < 400,
            FailureReason = entry.StatusCode is >= 400 ? $"HTTP {entry.StatusCode}" : null
        };

        await tokenRepository.UpdateLastUsedAtAsync(tokenId, now, cancellationToken);
        await tokenRepository.InsertUsageLogAsync(item, cancellationToken);
    }
}
