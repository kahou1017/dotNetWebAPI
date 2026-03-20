using Dimensions.Application.Interfaces;

namespace Dimensions.Application.Services;

public sealed class ApiLogService : IApiLogService
{
    public Task LogRequestAsync(string path, string method, string caseId, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
