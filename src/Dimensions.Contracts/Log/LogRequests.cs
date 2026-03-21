namespace Dimensions.Contracts.Log;

public sealed record ApiRequestLogListRequest
{
    public string? CaseId { get; init; }
    public string? Path { get; init; }
    public string? UserId { get; init; }
    public string? TokenId { get; init; }
    public string? DeviceId { get; init; }
    public string? ClientIp { get; init; }
    public int? StatusCode { get; init; }
    public bool? IsAuthenticated { get; init; }
    public bool? IsSuccess { get; init; }
    public DateTimeOffset? RequestTimeStart { get; init; }
    public DateTimeOffset? RequestTimeEnd { get; init; }
    public int PageNo { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed record ApiExceptionLogListRequest
{
    public string? CaseId { get; init; }
    public string? Path { get; init; }
    public string? UserId { get; init; }
    public string? TokenId { get; init; }
    public string? DeviceId { get; init; }
    public string? ClientIp { get; init; }
    public int? StatusCode { get; init; }
    public string? ErrorCode { get; init; }
    public string? ExceptionType { get; init; }
    public DateTimeOffset? OccurredAtStart { get; init; }
    public DateTimeOffset? OccurredAtEnd { get; init; }
    public int PageNo { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
