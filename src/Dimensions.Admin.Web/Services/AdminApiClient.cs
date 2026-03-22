using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Dimensions.Admin.Web.Models;
using Dimensions.Contracts.Auth;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Device;
using Dimensions.Contracts.Log;
using Dimensions.Contracts.Token;

namespace Dimensions.Admin.Web.Services;

public sealed class AdminApiClient(HttpClient httpClient, IAdminSessionAccessor adminSessionAccessor) : IAdminApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public Task<AdminApiResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        => PostAsync<LoginRequest, LoginResponse>("admin-api/auth/login", request, authorize: false, cancellationToken);

    public Task<AdminApiResult<CurrentAdminResponse>> GetCurrentAdminAsync(CancellationToken cancellationToken = default)
        => GetAsync<CurrentAdminResponse>("admin-api/auth/me", cancellationToken);

    public Task<AdminApiResult<PagedResult<TokenListItemResponse>>> GetTokenListAsync(TokenListRequest request, CancellationToken cancellationToken = default)
        => PostAsync<TokenListRequest, PagedResult<TokenListItemResponse>>("admin-api/token/list", request, authorize: true, cancellationToken);

    public Task<AdminApiResult<TokenDetailResponse>> GetTokenDetailAsync(TokenDetailRequest request, CancellationToken cancellationToken = default)
        => PostAsync<TokenDetailRequest, TokenDetailResponse>("admin-api/token/detail", request, authorize: true, cancellationToken);

    public Task<AdminApiResult<CreateTokenResponse>> CreateTokenAsync(CreateTokenRequest request, CancellationToken cancellationToken = default)
        => PostAsync<CreateTokenRequest, CreateTokenResponse>("admin-api/token/create", request, authorize: true, cancellationToken);

    public Task<AdminApiResult<RevokeTokenResponse>> RevokeTokenAsync(RevokeTokenRequest request, CancellationToken cancellationToken = default)
        => PostAsync<RevokeTokenRequest, RevokeTokenResponse>("admin-api/token/revoke", request, authorize: true, cancellationToken);

    public Task<AdminApiResult<ReissueTokenResponse>> ReissueTokenAsync(ReissueTokenRequest request, CancellationToken cancellationToken = default)
        => PostAsync<ReissueTokenRequest, ReissueTokenResponse>("admin-api/token/reissue", request, authorize: true, cancellationToken);

    public Task<AdminApiResult<RenewTokenResponse>> RenewTokenAsync(RenewTokenRequest request, CancellationToken cancellationToken = default)
        => PostAsync<RenewTokenRequest, RenewTokenResponse>("admin-api/token/renew", request, authorize: true, cancellationToken);

    public Task<AdminApiResult<PagedResult<TokenUsageItemResponse>>> GetTokenUsageAsync(TokenUsageRequest request, CancellationToken cancellationToken = default)
        => PostAsync<TokenUsageRequest, PagedResult<TokenUsageItemResponse>>("admin-api/token/usage", request, authorize: true, cancellationToken);

    public Task<AdminApiResult<PagedResult<TokenActionLogItemResponse>>> GetTokenActionLogsAsync(TokenActionLogRequest request, CancellationToken cancellationToken = default)
        => PostAsync<TokenActionLogRequest, PagedResult<TokenActionLogItemResponse>>("admin-api/token/action-log", request, authorize: true, cancellationToken);

    public Task<AdminApiResult<PagedResult<DeviceListItemResponse>>> GetDevicesAsync(DeviceListRequest request, CancellationToken cancellationToken = default)
        => PostAsync<DeviceListRequest, PagedResult<DeviceListItemResponse>>("admin-api/device/list", request, authorize: true, cancellationToken);

    public Task<AdminApiResult<object>> CreateDeviceAsync(CreateDeviceRequest request, CancellationToken cancellationToken = default)
        => PostAsync<CreateDeviceRequest, object>("admin-api/device/create", request, authorize: true, cancellationToken);

    public Task<AdminApiResult<object>> DisableDeviceAsync(DisableDeviceRequest request, CancellationToken cancellationToken = default)
        => PostAsync<DisableDeviceRequest, object>("admin-api/device/disable", request, authorize: true, cancellationToken);

    public Task<AdminApiResult<PagedResult<ApiRequestLogItemResponse>>> GetRequestLogsAsync(ApiRequestLogListRequest request, CancellationToken cancellationToken = default)
        => PostAsync<ApiRequestLogListRequest, PagedResult<ApiRequestLogItemResponse>>("admin-api/log/request/list", request, authorize: true, cancellationToken);

    public Task<AdminApiResult<PagedResult<ApiExceptionLogItemResponse>>> GetExceptionLogsAsync(ApiExceptionLogListRequest request, CancellationToken cancellationToken = default)
        => PostAsync<ApiExceptionLogListRequest, PagedResult<ApiExceptionLogItemResponse>>("admin-api/log/exception/list", request, authorize: true, cancellationToken);

    private async Task<AdminApiResult<TData>> GetAsync<TData>(string path, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        ApplyAuthorization(request, authorize: true);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        return await ParseResponseAsync<TData>(response, cancellationToken);
    }

    private async Task<AdminApiResult<TData>> PostAsync<TRequest, TData>(string path, TRequest payload, bool authorize, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = JsonContent.Create(payload)
        };

        ApplyAuthorization(request, authorize);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        return await ParseResponseAsync<TData>(response, cancellationToken);
    }

    private void ApplyAuthorization(HttpRequestMessage request, bool authorize)
    {
        if (!authorize)
        {
            return;
        }

        var session = adminSessionAccessor.GetCurrent();
        if (!string.IsNullOrWhiteSpace(session?.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        }
    }

    private static async Task<AdminApiResult<TData>> ParseResponseAsync<TData>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        ApiResponse<TData>? apiResponse = null;
        try
        {
            apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TData>>(JsonOptions, cancellationToken);
        }
        catch
        {
            // Ignore parse failures and fall back to generic error.
        }

        if (response.IsSuccessStatusCode && apiResponse is not null)
        {
            return new AdminApiResult<TData>
            {
                IsSuccess = apiResponse.Success,
                StatusCode = (int)response.StatusCode,
                CaseId = apiResponse.CaseId,
                Data = apiResponse.Data,
                ErrorCode = apiResponse.Error?.ErrorCode,
                ErrorMessage = apiResponse.Error?.ErrorMessage
            };
        }

        return new AdminApiResult<TData>
        {
            IsSuccess = false,
            StatusCode = (int)response.StatusCode,
            CaseId = apiResponse?.CaseId,
            ErrorCode = apiResponse?.Error?.ErrorCode ?? GetDefaultErrorCode(response.StatusCode),
            ErrorMessage = apiResponse?.Error?.ErrorMessage ?? $"Admin API request failed with status {(int)response.StatusCode}."
        };
    }

    private static string GetDefaultErrorCode(HttpStatusCode statusCode)
        => statusCode switch
        {
            HttpStatusCode.Unauthorized => "Auth.Unauthorized",
            HttpStatusCode.Forbidden => "Auth.Forbidden",
            HttpStatusCode.NotFound => "Resource.NotFound",
            _ => "AdminApi.RequestFailed"
        };
}
