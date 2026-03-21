using Dimensions.Contracts.Common;
using Dimensions.Contracts.Log;

namespace Dimensions.Application.Interfaces;

public interface ILogService
{
    Task<PagedResult<ApiRequestLogItemResponse>> GetApiRequestLogsAsync(ApiRequestLogListRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<ApiExceptionLogItemResponse>> GetApiExceptionLogsAsync(ApiExceptionLogListRequest request, CancellationToken cancellationToken = default);
}
