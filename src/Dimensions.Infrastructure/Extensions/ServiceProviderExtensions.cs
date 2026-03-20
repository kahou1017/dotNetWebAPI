using Dimensions.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Dimensions.Infrastructure.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task InitializeDimensionsDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        await initializer.InitializeAsync(cancellationToken);
    }
}
