using System.Text.Json;
using Dimensions.Api.Responses;

namespace Dimensions.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IConfiguration configuration)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var systemCode = configuration["System:SystemCode"] ?? "Dimensions";
            var payload = ApiResponseFactory.Error(context, systemCode, "System.InternalError", "An unexpected error occurred.");
            await JsonSerializer.SerializeAsync(context.Response.Body, payload, cancellationToken: context.RequestAborted);
        }
    }
}
