using Dimensions.Application.Models;

namespace Dimensions.Application.Interfaces;

public interface IApiLogService
{
    Task LogRequestAsync(ApiRequestLogEntry entry, CancellationToken cancellationToken = default);
}
