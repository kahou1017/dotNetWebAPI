using Dimensions.Application.Models;

namespace Dimensions.Application.Interfaces;

public interface IApiPayloadLogService
{
    Task LogAsync(ApiPayloadLogEntry entry, CancellationToken cancellationToken = default);
}
