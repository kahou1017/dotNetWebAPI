using System.Text;
using Dimensions.Api.Options;
using Dimensions.Api.Policies;
using Dimensions.Domain.Constants;
using Dimensions.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Dimensions.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddDimensionsAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), $"{JwtOptions.SectionName}:Issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), $"{JwtOptions.SectionName}:Audience is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.SecretKey), $"{JwtOptions.SectionName}:SecretKey is required.")
            .Validate(options => options.DefaultExpireDays > 0, $"{JwtOptions.SectionName}:DefaultExpireDays must be greater than zero.")
            .ValidateOnStart();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    RequireExpirationTime = false,
                    NameClaimType = ClaimNames.Name,
                    RoleClaimType = ClaimNames.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var tokenRepository = context.HttpContext.RequestServices.GetRequiredService<Dimensions.Application.Interfaces.ITokenRepository>();
                        var tokenId = context.Principal?.FindFirst(ClaimNames.TokenId)?.Value;
                        var jwtId = context.Principal?.FindFirst(ClaimNames.JwtId)?.Value;
                        var tokenType = context.Principal?.FindFirst(ClaimNames.TokenType)?.Value;

                        if (string.IsNullOrWhiteSpace(tokenId)
                            || string.IsNullOrWhiteSpace(jwtId)
                            || string.IsNullOrWhiteSpace(tokenType))
                        {
                            context.Fail("Required token claims are missing.");
                            return;
                        }

                        var token = await tokenRepository.GetValidTokenAsync(tokenId, jwtId, tokenType, context.HttpContext.RequestAborted);
                        if (token is null)
                        {
                            context.Fail("Token is not active.");
                        }
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.AdminOnly, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.HasClaim(ClaimNames.TokenType, TokenType.AdminSession)
                    && context.User.HasClaim(ClaimNames.Role, "Admin"));
            });

            options.AddPolicy(PolicyNames.TokenManage, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.HasClaim(ClaimNames.TokenType, TokenType.AdminSession)
                    && context.User.Claims.Any(claim =>
                        claim.Type == ClaimNames.Scope
                        && claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Contains("token.manage", StringComparer.OrdinalIgnoreCase)));
            });

            options.AddPolicy(PolicyNames.AuthenticatedUser, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.Claims.Any(claim =>
                        claim.Type == ClaimNames.TokenType
                        && (claim.Value == TokenType.UserAccess
                            || claim.Value == TokenType.Integration
                            || claim.Value == TokenType.Service)));
            });
        });

        return services;
    }
}
