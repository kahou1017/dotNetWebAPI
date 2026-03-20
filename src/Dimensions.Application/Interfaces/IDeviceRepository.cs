using Dimensions.Application.Models;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Device;

namespace Dimensions.Application.Interfaces;

public interface IDeviceRepository
{
    Task<PagedResult<DeviceListItemResponse>> GetDeviceListAsync(DeviceListRequest request, CancellationToken cancellationToken = default);

    Task<DeviceListItemResponse> UpsertDeviceAsync(DeviceWriteModel device, CancellationToken cancellationToken = default);

    Task DisableDeviceAsync(string deviceId, CancellationToken cancellationToken = default);
}
