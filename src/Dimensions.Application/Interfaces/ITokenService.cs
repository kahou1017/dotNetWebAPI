using Dimensions.Contracts.Common;
using Dimensions.Contracts.Token;

namespace Dimensions.Application.Interfaces;

public interface ITokenService
{
    Task<PagedResult<TokenListItemResponse>> GetTokenListAsync(TokenListRequest request, CancellationToken cancellationToken = default);

    Task<TokenDetailResponse> GetTokenDetailAsync(TokenDetailRequest request, CancellationToken cancellationToken = default);

    Task<CreateTokenResponse> CreateTokenAsync(CreateTokenRequest request, CancellationToken cancellationToken = default);

    Task<RevokeTokenResponse> RevokeTokenAsync(RevokeTokenRequest request, CancellationToken cancellationToken = default);

    Task<ReissueTokenResponse> ReissueTokenAsync(ReissueTokenRequest request, CancellationToken cancellationToken = default);

    Task<RenewTokenResponse> RenewTokenAsync(RenewTokenRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<TokenUsageItemResponse>> GetTokenUsageAsync(TokenUsageRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<TokenActionLogItemResponse>> GetTokenActionLogAsync(TokenActionLogRequest request, CancellationToken cancellationToken = default);
}
