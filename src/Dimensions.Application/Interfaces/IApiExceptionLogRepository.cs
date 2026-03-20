using Dimensions.Application.Models;

namespace Dimensions.Application.Interfaces;

public interface IApiExceptionLogRepository
{
    Task InsertAsync(ApiExceptionLogRecord record, CancellationToken cancellationToken = default);
}
