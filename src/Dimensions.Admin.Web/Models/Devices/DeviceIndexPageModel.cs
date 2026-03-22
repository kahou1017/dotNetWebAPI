using Dimensions.Contracts.Common;
using Dimensions.Contracts.Device;

namespace Dimensions.Admin.Web.Models.Devices;

public sealed class DeviceIndexPageModel
{
    public DeviceListRequest Filter { get; init; } = new();

    public PagedResult<DeviceListItemResponse>? Result { get; init; }

    public DisableDeviceFormModel DisableForm { get; init; } = new();

    public string? ResultMessage { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public string? CaseId { get; init; }
}
