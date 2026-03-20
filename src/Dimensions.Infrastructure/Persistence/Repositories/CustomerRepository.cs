using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;

namespace Dimensions.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository(DapperSqlExecutor sqlExecutor) : ICustomerRepository
{
    public Task<CustomerRecordModel?> GetByCustomerIdAsync(
        string customerId,
        string? keyword,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                CustomerId,
                CustomerName,
                Status
            FROM Customers
            WHERE CustomerId = @CustomerId
              AND (@Keyword IS NULL
                   OR CustomerName LIKE '%' || @Keyword || '%'
                   OR Status LIKE '%' || @Keyword || '%')
            LIMIT 1;
            """;

        return sqlExecutor.QuerySingleOrDefaultAsync<CustomerRecordModel>(
            sql,
            new
            {
                CustomerId = customerId,
                Keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim()
            },
            cancellationToken: cancellationToken);
    }
}
