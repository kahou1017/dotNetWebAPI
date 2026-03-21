using Dimensions.Application.Interfaces;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Log;

namespace Dimensions.Application.Services;

public sealed class LogService(
    IApiRequestLogRepository apiRequestLogRepository,
    IApiExceptionLogRepository apiExceptionLogRepository) : ILogService
{
    public Task<PagedResult<ApiRequestLogItemResponse>> GetApiRequestLogsAsync(
        ApiRequestLogListRequest request,
        CancellationToken cancellationToken = default)
        => apiRequestLogRepository.GetListAsync(request, cancellationToken);

    public Task<PagedResult<ApiExceptionLogItemResponse>> GetApiExceptionLogsAsync(
        ApiExceptionLogListRequest request,
        CancellationToken cancellationToken = default)
        => apiExceptionLogRepository.GetListAsync(request, cancellationToken);
}
