namespace Dimensions.Application.Models;

public sealed record ApiPayloadLogRecord
{
    public string PayloadLogId { get; init; } = string.Empty;

    public string CaseId { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }

    public string Direction { get; init; } = string.Empty;

    public string Method { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public string? ContentType { get; init; }

    public string PayloadText { get; init; } = string.Empty;

    public int PayloadLength { get; init; }

    public bool IsTruncated { get; init; }

    public bool IsAuthenticated { get; init; }

    public string? TokenId { get; init; }

    public string? UserId { get; init; }

    public string? TokenType { get; init; }
}
