namespace Dimensions.Application.Models;

public sealed record ApiRequestLogRecord
{
    public string CaseId { get; init; } = string.Empty;

    public DateTimeOffset RequestTime { get; init; }

    public string Method { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public int StatusCode { get; init; }

    public string? ClientIp { get; init; }

    public string? DeviceId { get; init; }

    public bool IsAuthenticated { get; init; }

    public bool IsSuccess { get; init; }

    public string? TokenId { get; init; }

    public string? UserId { get; init; }

    public string? TokenType { get; init; }
}
