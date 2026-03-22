using Dimensions.Contracts.Common;
using Dimensions.Contracts.Log;

namespace Dimensions.Admin.Web.Models.Logs;

public sealed class RequestLogsPageModel
{
    public ApiRequestLogListRequest Filter { get; init; } = new();

    public PagedResult<ApiRequestLogItemResponse>? Result { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public string? CaseId { get; init; }
}
