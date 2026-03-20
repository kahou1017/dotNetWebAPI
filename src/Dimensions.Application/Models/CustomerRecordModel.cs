namespace Dimensions.Application.Models;

public sealed record CustomerRecordModel
{
    public string CustomerId { get; init; } = string.Empty;

    public string CustomerName { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;
}
