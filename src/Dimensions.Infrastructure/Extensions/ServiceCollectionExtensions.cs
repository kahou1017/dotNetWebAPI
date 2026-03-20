using Dimensions.Application.Interfaces;
using Dimensions.Infrastructure.Options;
using Dimensions.Infrastructure.Persistence;
using Dimensions.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dimensions.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDimensionsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Provider),
                $"{DatabaseOptions.SectionName}:Provider is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.ConnectionStringName),
                $"{DatabaseOptions.SectionName}:ConnectionStringName is required.")
            .Validate(
                options => options.CommandTimeoutSeconds > 0,
                $"{DatabaseOptions.SectionName}:CommandTimeoutSeconds must be greater than zero.")
            .ValidateOnStart();

        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<DapperSqlExecutor>();
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();

        return services;
    }
}
