namespace Dimensions.Contracts.Device;

public sealed record DeviceListItemResponse
{
    public string UserId { get; init; } = string.Empty;
    public string DeviceId { get; init; } = string.Empty;
    public string DeviceName { get; init; } = string.Empty;
    public string DeviceType { get; init; } = string.Empty;
    public bool IsEnabled { get; init; }
}
