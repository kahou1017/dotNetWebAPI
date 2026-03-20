using Dimensions.Application.Models;

namespace Dimensions.Application.Interfaces;

public interface IApiExceptionLogService
{
    Task LogAsync(ApiExceptionLogEntry entry, CancellationToken cancellationToken = default);
}
