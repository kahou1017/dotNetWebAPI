namespace Dimensions.Application.Models;

public sealed record ApiPayloadLogEntry
{
    public string CaseId { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public string Direction { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public string Method { get; init; } = string.Empty;

    public string? ContentType { get; init; }

    public string PayloadText { get; init; } = string.Empty;

    public int PayloadLength { get; init; }

    public bool IsTruncated { get; init; }
}
