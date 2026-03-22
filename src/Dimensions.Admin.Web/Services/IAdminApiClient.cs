using Dimensions.Admin.Web.Models;
using Dimensions.Contracts.Auth;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Device;
using Dimensions.Contracts.Log;
using Dimensions.Contracts.Token;

namespace Dimensions.Admin.Web.Services;

public interface IAdminApiClient
{
    Task<AdminApiResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<AdminApiResult<CurrentAdminResponse>> GetCurrentAdminAsync(CancellationToken cancellationToken = default);

    Task<AdminApiResult<PagedResult<TokenListItemResponse>>> GetTokenListAsync(TokenListRequest request, CancellationToken cancellationToken = default);

    Task<AdminApiResult<TokenDetailResponse>> GetTokenDetailAsync(TokenDetailRequest request, CancellationToken cancellationToken = default);

    Task<AdminApiResult<CreateTokenResponse>> CreateTokenAsync(CreateTokenRequest request, CancellationToken cancellationToken = default);

    Task<AdminApiResult<RevokeTokenResponse>> RevokeTokenAsync(RevokeTokenRequest request, CancellationToken cancellationToken = default);

    Task<AdminApiResult<ReissueTokenResponse>> ReissueTokenAsync(ReissueTokenRequest request, CancellationToken cancellationToken = default);

    Task<AdminApiResult<RenewTokenResponse>> RenewTokenAsync(RenewTokenRequest request, CancellationToken cancellationToken = default);

    Task<AdminApiResult<PagedResult<TokenUsageItemResponse>>> GetTokenUsageAsync(TokenUsageRequest request, CancellationToken cancellationToken = default);

    Task<AdminApiResult<PagedResult<TokenActionLogItemResponse>>> GetTokenActionLogsAsync(TokenActionLogRequest request, CancellationToken cancellationToken = default);

    Task<AdminApiResult<PagedResult<DeviceListItemResponse>>> GetDevicesAsync(DeviceListRequest request, CancellationToken cancellationToken = default);

    Task<AdminApiResult<object>> CreateDeviceAsync(CreateDeviceRequest request, CancellationToken cancellationToken = default);

    Task<AdminApiResult<object>> DisableDeviceAsync(DisableDeviceRequest request, CancellationToken cancellationToken = default);

    Task<AdminApiResult<PagedResult<ApiRequestLogItemResponse>>> GetRequestLogsAsync(ApiRequestLogListRequest request, CancellationToken cancellationToken = default);

    Task<AdminApiResult<PagedResult<ApiExceptionLogItemResponse>>> GetExceptionLogsAsync(ApiExceptionLogListRequest request, CancellationToken cancellationToken = default);
}
