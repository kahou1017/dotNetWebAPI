using Dimensions.Application.Exceptions;
using Dimensions.Application.Interfaces;
using Dimensions.Contracts.Business;

namespace Dimensions.Application.Services;

public sealed class CustomerService(
    ICustomerRepository customerRepository,
    ICurrentUserAccessor currentUserAccessor) : ICustomerService
{
    public async Task<CustomerQueryResponse> QueryAsync(
        CustomerQueryRequest request,
        string? deviceId,
        CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.GetByCustomerIdAsync(
            request.CustomerId,
            request.Keyword,
            cancellationToken);

        if (customer is null)
        {
            throw new DimensionsApplicationException(404, "Customer.NotFound", "The specified customer was not found.");
        }

        return new CustomerQueryResponse
        {
            CustomerId = customer.CustomerId,
            CustomerName = customer.CustomerName,
            Status = customer.Status,
            QueriedByUserId = currentUserAccessor.GetUserId() ?? string.Empty,
            QueriedByName = currentUserAccessor.GetDisplayName() ?? string.Empty,
            DeviceId = deviceId,
            QueriedAt = DateTimeOffset.UtcNow
        };
    }
}
