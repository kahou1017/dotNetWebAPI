namespace Dimensions.Application.Models;

public sealed record ApiRequestLogEntry
{
    public string CaseId { get; init; } = string.Empty;

    public DateTimeOffset RequestTime { get; init; } = DateTimeOffset.UtcNow;

    public string Path { get; init; } = string.Empty;

    public string Method { get; init; } = string.Empty;

    public int StatusCode { get; init; }

    public string? ClientIp { get; init; }

    public string? DeviceId { get; init; }
}
