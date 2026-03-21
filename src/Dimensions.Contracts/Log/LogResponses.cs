namespace Dimensions.Contracts.Log;

public sealed record ApiRequestLogItemResponse
{
    public string CaseId { get; init; } = string.Empty;
    public DateTimeOffset RequestTime { get; init; }
    public string HttpMethod { get; init; } = string.Empty;
    public string RequestPath { get; init; } = string.Empty;
    public int StatusCode { get; init; }
    public string? ClientIp { get; init; }
    public string? DeviceId { get; init; }
    public bool IsAuthenticated { get; init; }
    public bool IsSuccess { get; init; }
    public string? TokenId { get; init; }
    public string? UserId { get; init; }
    public string? TokenType { get; init; }
}

public sealed record ApiExceptionLogItemResponse
{
    public string CaseId { get; init; } = string.Empty;
    public DateTimeOffset OccurredAt { get; init; }
    public string HttpMethod { get; init; } = string.Empty;
    public string RequestPath { get; init; } = string.Empty;
    public int StatusCode { get; init; }
    public string ExceptionType { get; init; } = string.Empty;
    public string? ErrorCode { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
    public string? ClientIp { get; init; }
    public string? DeviceId { get; init; }
    public bool IsAuthenticated { get; init; }
    public string? TokenId { get; init; }
    public string? UserId { get; init; }
    public string? TokenType { get; init; }
}
