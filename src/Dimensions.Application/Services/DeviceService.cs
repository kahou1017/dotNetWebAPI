using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Device;

namespace Dimensions.Application.Services;

public sealed class DeviceService(IDeviceRepository deviceRepository) : IDeviceService
{
    public Task<PagedResult<DeviceListItemResponse>> GetDeviceListAsync(DeviceListRequest request, CancellationToken cancellationToken = default)
        => deviceRepository.GetDeviceListAsync(request, cancellationToken);

    public Task<DeviceListItemResponse> CreateDeviceAsync(CreateDeviceRequest request, CancellationToken cancellationToken = default)
        => deviceRepository.UpsertDeviceAsync(
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

    public Task DisableDeviceAsync(DisableDeviceRequest request, CancellationToken cancellationToken = default)
        => deviceRepository.DisableDeviceAsync(request.DeviceId, cancellationToken);
}
