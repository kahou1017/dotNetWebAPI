using Microsoft.OpenApi.Models;

namespace Dimensions.Admin.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddDimensionsSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Dimensions Admin API",
                Version = "v1",
                Description = "Dimensions admin management API with JWT, SQLite, and log4net."
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Input: Bearer {your JWT token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Bearer", securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [securityScheme] = Array.Empty<string>()
            });
        });

        return services;
    }

    public static IApplicationBuilder UseDimensionsSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dimensions Admin API v1");
            options.DocumentTitle = "Dimensions Admin API Swagger";
            options.DisplayRequestDuration();
        });

        return app;
    }
}

