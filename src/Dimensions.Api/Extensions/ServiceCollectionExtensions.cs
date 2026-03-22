using Dimensions.Api.Services;
using Dimensions.Api.Validation;
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
        services.AddScoped<ValidationActionFilter>();
        services.AddScoped<ICaseIdAccessor, HttpContextCaseIdAccessor>();
        services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IApiExceptionLogService, ApiExceptionLogService>();
        services.AddScoped<IApiPayloadLogService, ApiPayloadLogService>();
        services.AddScoped<IApiRequestLogService, ApiRequestLogService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<ITokenUsageLogService, TokenUsageLogService>();

        return services;
    }
}
