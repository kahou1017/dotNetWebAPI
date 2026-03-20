using Dimensions.Application.Models;

namespace Dimensions.Application.Interfaces;

public interface ICustomerRepository
{
    Task<CustomerRecordModel?> GetByCustomerIdAsync(
        string customerId,
        string? keyword,
        CancellationToken cancellationToken = default);
}
