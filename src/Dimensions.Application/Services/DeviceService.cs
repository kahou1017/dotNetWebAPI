using Dimensions.Application.Exceptions;
using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Device;
using Dimensions.Contracts.Token;
using Dimensions.Domain.Enums;

namespace Dimensions.Application.Services;

public sealed class DeviceService(
    IDeviceRepository deviceRepository,
    ITokenRepository tokenRepository,
    ICaseIdAccessor caseIdAccessor,
    ICurrentUserAccessor currentUserAccessor) : IDeviceService
{
    public Task<PagedResult<DeviceListItemResponse>> GetDeviceListAsync(DeviceListRequest request, CancellationToken cancellationToken = default)
        => deviceRepository.GetDeviceListAsync(request, cancellationToken);

    public async Task<DeviceListItemResponse> CreateDeviceAsync(CreateDeviceRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await deviceRepository.GetDeviceByIdAsync(request.DeviceId, cancellationToken);
        if (existing is not null
            && !string.Equals(existing.UserId, request.UserId, StringComparison.Ordinal))
        {
            throw new DimensionsApplicationException(
                409,
                "Device.AlreadyBound",
                "This device id is already bound to another user.");
        }

        return await deviceRepository.UpsertDeviceAsync(
            new DeviceWriteModel
            {
                UserId = request.UserId,
                DeviceId = request.DeviceId,
                DeviceName = request.DeviceName,
                DeviceType = request.DeviceType,
                IsEnabled = true,
                Remark = request.Remark,
                UpdatedAt = DateTimeOffset.UtcNow
            },
            cancellationToken);
    }

    public async Task DisableDeviceAsync(DisableDeviceRequest request, CancellationToken cancellationToken = default)
    {
        var device = await deviceRepository.GetDeviceByIdAsync(request.DeviceId, cancellationToken);
        if (device is null)
        {
            throw new DimensionsApplicationException(
                404,
                "Device.NotFound",
                "The specified device does not exist.");
        }

        await deviceRepository.DisableDeviceAsync(request.DeviceId, cancellationToken);

        var boundTokens = await tokenRepository.GetActiveSingleDeviceTokensByDeviceAsync(device.UserId, device.DeviceId, cancellationToken);
        foreach (var token in boundTokens)
        {
            await tokenRepository.UpdateTokenStatusAsync(
                token.TokenId,
                TokenStatus.Disabled,
                isRevoked: false,
                revokedAt: null,
                expireAt: null,
                cancellationToken);

            await tokenRepository.InsertActionLogAsync(
                new TokenActionLogItemResponse
                {
                    CaseId = $"{caseIdAccessor.GetCaseId()}-{token.TokenId}",
                    TokenId = token.TokenId,
                    ActionType = "DisableDevice",
                    ActionReason = request.Reason,
                    ActionResult = "Success",
                    OperatorUserId = currentUserAccessor.GetUserId() ?? "ADMIN001",
                    OperatorUserName = currentUserAccessor.GetDisplayName() ?? "System Admin",
                    BeforeStatus = token.Status,
                    AfterStatus = TokenStatus.Disabled,
                    CreatedAt = DateTimeOffset.UtcNow
                },
                cancellationToken);
        }
    }
}
