namespace Dimensions.Application.Models;

public sealed record ApiExceptionLogRecord
{
    public string CaseId { get; init; } = string.Empty;

    public DateTimeOffset OccurredAt { get; init; }

    public string Method { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public int StatusCode { get; init; }

    public string ExceptionType { get; init; } = string.Empty;

    public string? ErrorCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? ClientIp { get; init; }

    public string? DeviceId { get; init; }

    public bool IsAuthenticated { get; init; }

    public string? TokenId { get; init; }

    public string? UserId { get; init; }

    public string? TokenType { get; init; }
}
