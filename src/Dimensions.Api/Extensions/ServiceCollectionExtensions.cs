using Dimensions.Api.Services;
using Dimensions.Application.Interfaces;
using Dimensions.Application.Services;
using Dimensions.Infrastructure.Extensions;

namespace Dimensions.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDimensionsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDimensionsInfrastructure(configuration);
        services.AddHttpContextAccessor();
        services.AddScoped<ICaseIdAccessor, HttpContextCaseIdAccessor>();
        services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<IApiLogService, ApiLogService>();

        return services;
    }
}
