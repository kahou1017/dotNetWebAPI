namespace Dimensions.Application.Interfaces;

public interface IApiLogService
{
    Task LogRequestAsync(string path, string method, string caseId, CancellationToken cancellationToken = default);
}
