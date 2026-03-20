namespace Dimensions.Application.Models;

public sealed record DeviceWriteModel
{
    public string UserId { get; init; } = string.Empty;

    public string DeviceId { get; init; } = string.Empty;

    public string DeviceName { get; init; } = string.Empty;

    public string DeviceType { get; init; } = string.Empty;

    public bool IsEnabled { get; init; }

    public string? Remark { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }
}
