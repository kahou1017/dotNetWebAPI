using Dimensions.Application.Models;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Log;

namespace Dimensions.Application.Interfaces;

public interface IApiExceptionLogRepository
{
    Task InsertAsync(ApiExceptionLogRecord record, CancellationToken cancellationToken = default);

    Task<PagedResult<ApiExceptionLogItemResponse>> GetListAsync(ApiExceptionLogListRequest request, CancellationToken cancellationToken = default);
}
