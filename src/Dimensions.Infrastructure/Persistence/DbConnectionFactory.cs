using System.Data;
using Dimensions.Application.Interfaces;
using Dimensions.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Dimensions.Infrastructure.Persistence;

public sealed class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseOptions _options;

    public DbConnectionFactory(IConfiguration configuration, IOptions<DatabaseOptions> options)
    {
        _configuration = configuration;
        _options = options.Value;
    }

    public IDbConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString(_options.ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{_options.ConnectionStringName}' is not configured.");
        }

        return _options.Provider.Trim().ToLowerInvariant() switch
        {
            "sqlserver" => new SqlConnection(connectionString),
            "sqlite" => new SqliteConnection(connectionString),
            _ => throw new InvalidOperationException(
                $"Unsupported database provider '{_options.Provider}'. Supported providers: SqlServer, Sqlite.")
        };
    }
}
