using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;

namespace Dimensions.Application.Services;

public sealed class ApiRequestLogService(
    IApiRequestLogRepository apiRequestLogRepository,
    ICurrentUserAccessor currentUserAccessor) : IApiRequestLogService
{
    public Task LogAsync(ApiRequestLogEntry entry, CancellationToken cancellationToken = default)
    {
        var tokenId = currentUserAccessor.GetTokenId();
        var userId = currentUserAccessor.GetUserId();
        var tokenType = currentUserAccessor.GetTokenType();

        return apiRequestLogRepository.InsertAsync(
            new ApiRequestLogRecord
            {
                CaseId = entry.CaseId,
                RequestTime = entry.RequestTime,
                Method = entry.Method,
                Path = entry.Path,
                StatusCode = entry.StatusCode,
                ClientIp = entry.ClientIp,
                DeviceId = entry.DeviceId,
                IsAuthenticated = !string.IsNullOrWhiteSpace(tokenId) && !string.IsNullOrWhiteSpace(userId),
                IsSuccess = entry.StatusCode is >= 200 and < 400,
                TokenId = tokenId,
                UserId = userId,
                TokenType = tokenType
            },
            cancellationToken);
    }
}
