using Dimensions.Contracts.Business;

namespace Dimensions.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerQueryResponse> QueryAsync(
        CustomerQueryRequest request,
        string? deviceId,
        CancellationToken cancellationToken = default);
}
