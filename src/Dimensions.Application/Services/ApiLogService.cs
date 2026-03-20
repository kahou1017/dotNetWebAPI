using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Contracts.Token;

namespace Dimensions.Application.Services;

public sealed class ApiLogService(
    ICurrentUserAccessor currentUserAccessor,
    ITokenRepository tokenRepository) : IApiLogService
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
