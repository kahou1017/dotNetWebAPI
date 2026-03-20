namespace Dimensions.Application.Models;

public sealed record ApiExceptionLogEntry
{
    public string CaseId { get; init; } = string.Empty;

    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    public string Path { get; init; } = string.Empty;

    public string Method { get; init; } = string.Empty;

    public int StatusCode { get; init; }

    public string ExceptionType { get; init; } = string.Empty;

    public string? ErrorCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? ClientIp { get; init; }

    public string? DeviceId { get; init; }
}
