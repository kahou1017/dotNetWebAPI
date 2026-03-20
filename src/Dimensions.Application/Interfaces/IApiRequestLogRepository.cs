using Dimensions.Application.Models;

namespace Dimensions.Application.Interfaces;

public interface IApiRequestLogRepository
{
    Task InsertAsync(ApiRequestLogRecord record, CancellationToken cancellationToken = default);
}
