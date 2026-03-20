namespace Dimensions.Contracts.Business;

public sealed record CustomerQueryRequest
{
    public string CustomerId { get; init; } = string.Empty;

    public string? Keyword { get; init; }
}
