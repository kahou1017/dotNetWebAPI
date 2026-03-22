namespace Dimensions.Admin.Web.Models;

public sealed record AdminApiResult<T>
{
    public bool IsSuccess { get; init; }

    public int StatusCode { get; init; }

    public string? CaseId { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public T? Data { get; init; }
}
