namespace Dimensions.Contracts.Common;

public sealed record PagedResult<T>
{
    public required int PageNo { get; init; }

    public required int PageSize { get; init; }

    public required int TotalCount { get; init; }

    public required IReadOnlyList<T> Items { get; init; }
}
