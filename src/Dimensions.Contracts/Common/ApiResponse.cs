namespace Dimensions.Contracts.Common;

public sealed record ApiResponse<T>
{
    public required DateTimeOffset Timestamp { get; init; }

    public required bool Success { get; init; }

    public required string Method { get; init; }

    public required string Path { get; init; }

    public required string CaseId { get; init; }

    public required string SystemCode { get; init; }

    public T? Data { get; init; }

    public ApiErrorData? Error { get; init; }
}
