using Dimensions.Application.Models;

namespace Dimensions.Application.Interfaces;

public interface IApiPayloadLogRepository
{
    Task InsertAsync(ApiPayloadLogRecord record, CancellationToken cancellationToken = default);
}
