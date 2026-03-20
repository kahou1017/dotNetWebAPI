using Dimensions.Contracts.Common;
using Dimensions.Contracts.Device;

namespace Dimensions.Application.Interfaces;

public interface IDeviceService
{
    Task<PagedResult<DeviceListItemResponse>> GetDeviceListAsync(DeviceListRequest request, CancellationToken cancellationToken = default);

    Task<DeviceListItemResponse> CreateDeviceAsync(CreateDeviceRequest request, CancellationToken cancellationToken = default);

    Task DisableDeviceAsync(DisableDeviceRequest request, CancellationToken cancellationToken = default);
}
