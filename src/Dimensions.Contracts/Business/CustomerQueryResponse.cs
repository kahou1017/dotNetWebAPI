namespace Dimensions.Contracts.Business;

public sealed record CustomerQueryResponse
{
    public string CustomerId { get; init; } = string.Empty;

    public string CustomerName { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string QueriedByUserId { get; init; } = string.Empty;

    public string QueriedByName { get; init; } = string.Empty;

    public string? DeviceId { get; init; }

    public DateTimeOffset QueriedAt { get; init; }
}
