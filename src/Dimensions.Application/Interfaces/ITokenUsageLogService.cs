using Dimensions.Application.Models;

namespace Dimensions.Application.Interfaces;

public interface ITokenUsageLogService
{
    Task LogRequestAsync(ApiRequestLogEntry entry, CancellationToken cancellationToken = default);
}
