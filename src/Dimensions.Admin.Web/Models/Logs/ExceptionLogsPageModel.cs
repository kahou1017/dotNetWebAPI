using Dimensions.Contracts.Common;
using Dimensions.Contracts.Log;

namespace Dimensions.Admin.Web.Models.Logs;

public sealed class ExceptionLogsPageModel
{
    public ApiExceptionLogListRequest Filter { get; init; } = new();

    public PagedResult<ApiExceptionLogItemResponse>? Result { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public string? CaseId { get; init; }
}
