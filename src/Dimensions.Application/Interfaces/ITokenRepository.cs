using Dimensions.Application.Models;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Token;

namespace Dimensions.Application.Interfaces;

public interface ITokenRepository
{
    Task<PagedResult<TokenListItemResponse>> GetTokenListAsync(TokenListRequest request, CancellationToken cancellationToken = default);

    Task<TokenDetailResponse?> GetTokenDetailAsync(string tokenId, CancellationToken cancellationToken = default);

    Task<TokenDetailResponse?> GetValidTokenAsync(
        string tokenId,
        string jwtId,
        string tokenType,
        CancellationToken cancellationToken = default);

    Task<TokenDetailResponse?> GetTokenByJwtAsync(
        string tokenId,
        string jwtId,
        string tokenType,
        CancellationToken cancellationToken = default);

    Task InsertTokenAsync(TokenWriteModel token, CancellationToken cancellationToken = default);

    Task UpdateTokenStatusAsync(
        string tokenId,
        string status,
        bool isRevoked,
        DateTimeOffset? revokedAt,
        DateTimeOffset? expireAt,
        CancellationToken cancellationToken = default);

    Task UpdateTokenCredentialsAsync(
        string tokenId,
        string jwtId,
        string accessToken,
        DateTimeOffset issuedAt,
        DateTimeOffset effectiveAt,
        DateTimeOffset? expireAt,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TokenDetailResponse>> GetActiveSingleDeviceTokensByDeviceAsync(
        string userId,
        string deviceId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<TokenUsageItemResponse>> GetTokenUsageAsync(TokenUsageRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<TokenActionLogItemResponse>> GetTokenActionLogAsync(TokenActionLogRequest request, CancellationToken cancellationToken = default);

    Task InsertActionLogAsync(TokenActionLogItemResponse item, CancellationToken cancellationToken = default);

    Task UpdateLastUsedAtAsync(string tokenId, DateTimeOffset lastUsedAt, CancellationToken cancellationToken = default);

    Task InsertUsageLogAsync(TokenUsageItemResponse item, CancellationToken cancellationToken = default);
}
