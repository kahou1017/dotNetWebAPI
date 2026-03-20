using Dimensions.Application.Models;

namespace Dimensions.Application.Interfaces;

public interface IApiRequestLogService
{
    Task LogAsync(ApiRequestLogEntry entry, CancellationToken cancellationToken = default);
}
