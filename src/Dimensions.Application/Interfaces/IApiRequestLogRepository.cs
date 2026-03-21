using Dimensions.Application.Models;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Log;

namespace Dimensions.Application.Interfaces;

public interface IApiRequestLogRepository
{
    Task InsertAsync(ApiRequestLogRecord record, CancellationToken cancellationToken = default);

    Task<PagedResult<ApiRequestLogItemResponse>> GetListAsync(ApiRequestLogListRequest request, CancellationToken cancellationToken = default);
}
