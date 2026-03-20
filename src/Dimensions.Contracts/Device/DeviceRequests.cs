namespace Dimensions.Contracts.Device;

public sealed record DeviceListRequest
{
    public string? UserId { get; init; }
    public string? DeviceId { get; init; }
    public bool? IsEnabled { get; init; }
    public int PageNo { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed record CreateDeviceRequest
{
    public string UserId { get; init; } = string.Empty;
    public string DeviceId { get; init; } = string.Empty;
    public string DeviceName { get; init; } = string.Empty;
    public string DeviceType { get; init; } = string.Empty;
    public string? Remark { get; init; }
}

public sealed record DisableDeviceRequest
{
    public string DeviceId { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}
