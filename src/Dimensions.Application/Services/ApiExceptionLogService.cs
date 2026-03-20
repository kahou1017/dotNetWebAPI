using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;

namespace Dimensions.Application.Services;

public sealed class ApiExceptionLogService(
    IApiExceptionLogRepository apiExceptionLogRepository,
    ICurrentUserAccessor currentUserAccessor) : IApiExceptionLogService
{
    public Task LogAsync(ApiExceptionLogEntry entry, CancellationToken cancellationToken = default)
    {
        var tokenId = currentUserAccessor.GetTokenId();
        var userId = currentUserAccessor.GetUserId();
        var tokenType = currentUserAccessor.GetTokenType();

        return apiExceptionLogRepository.InsertAsync(
            new ApiExceptionLogRecord
            {
                CaseId = entry.CaseId,
                OccurredAt = entry.OccurredAt,
                Method = entry.Method,
                Path = entry.Path,
                StatusCode = entry.StatusCode,
                ExceptionType = entry.ExceptionType,
                ErrorCode = entry.ErrorCode,
                Message = entry.Message,
                ClientIp = entry.ClientIp,
                DeviceId = entry.DeviceId,
                IsAuthenticated = !string.IsNullOrWhiteSpace(tokenId) && !string.IsNullOrWhiteSpace(userId),
                TokenId = tokenId,
                UserId = userId,
                TokenType = tokenType
            },
            cancellationToken);
    }
}
