using System.Data;
using Dapper;
using Dimensions.Application.Interfaces;
using Dimensions.Infrastructure.Options;
using Dimensions.Infrastructure.Persistence.TypeHandlers;
using Microsoft.Extensions.Options;

namespace Dimensions.Infrastructure.Persistence;

public sealed class DapperSqlExecutor
{
    private static int _typeHandlersInitialized;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly DatabaseOptions _options;

    public DapperSqlExecutor(IDbConnectionFactory connectionFactory, IOptions<DatabaseOptions> options)
    {
        EnsureTypeHandlers();
        _connectionFactory = connectionFactory;
        _options = options.Value;
    }

    public async Task<int> ExecuteAsync(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteAsync(new CommandDefinition(
            sql,
            parameters,
            commandType: commandType,
            commandTimeout: _options.CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var results = await connection.QueryAsync<T>(new CommandDefinition(
            sql,
            parameters,
            commandType: commandType,
            commandTimeout: _options.CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return results.AsList();
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(
            sql,
            parameters,
            commandType: commandType,
            commandTimeout: _options.CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    private static void EnsureTypeHandlers()
    {
        if (Interlocked.Exchange(ref _typeHandlersInitialized, 1) == 1)
        {
            return;
        }

        SqlMapper.AddTypeHandler(new SqliteDateTimeOffsetTypeHandler());
        SqlMapper.AddTypeHandler(new NullableSqliteDateTimeOffsetTypeHandler());
    }
}
