using System.Text.Json;
using Dimensions.Application.Interfaces;
using Dimensions.Application.Exceptions;
using Dimensions.Application.Models;
using Dimensions.Api.Responses;
using Dimensions.Domain.Constants;

namespace Dimensions.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IConfiguration configuration)
{
    public async Task InvokeAsync(HttpContext context, IApiExceptionLogService apiExceptionLogService)
    {
        try
        {
            await next(context);
        }
        catch (DimensionsApplicationException ex)
        {
            logger.LogWarning(
                "Handled application exception {ErrorCode} while processing {Method} {Path}: {Message}",
                ex.ErrorCode,
                context.Request.Method,
                context.Request.Path,
                ex.ErrorMessage);

            await TryWriteExceptionLogAsync(
                context,
                apiExceptionLogService,
                ex.GetType().Name,
                ex.ErrorCode,
                ex.ErrorMessage,
                ex.StatusCode);

            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";

            var systemCode = configuration["System:SystemCode"] ?? "Dimensions";
            var payload = ApiResponseFactory.Error(context, systemCode, ex.ErrorCode, ex.ErrorMessage);
            await JsonSerializer.SerializeAsync(context.Response.Body, payload, cancellationToken: context.RequestAborted);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);

            await TryWriteExceptionLogAsync(
                context,
                apiExceptionLogService,
                ex.GetType().Name,
                "System.InternalError",
                ex.Message,
                StatusCodes.Status500InternalServerError);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var systemCode = configuration["System:SystemCode"] ?? "Dimensions";
            var payload = ApiResponseFactory.Error(context, systemCode, "System.InternalError", "An unexpected error occurred.");
            await JsonSerializer.SerializeAsync(context.Response.Body, payload, cancellationToken: context.RequestAborted);
        }
    }

    private async Task TryWriteExceptionLogAsync(
        HttpContext context,
        IApiExceptionLogService apiExceptionLogService,
        string exceptionType,
        string? errorCode,
        string message,
        int statusCode)
    {
        try
        {
            await apiExceptionLogService.LogAsync(
                new ApiExceptionLogEntry
                {
                    CaseId = ApiResponseFactory.GetCaseId(context),
                    OccurredAt = DateTimeOffset.UtcNow,
                    Path = context.Request.Path.Value ?? string.Empty,
                    Method = context.Request.Method,
                    StatusCode = statusCode,
                    ExceptionType = exceptionType,
                    ErrorCode = errorCode,
                    Message = message,
                    ClientIp = context.Connection.RemoteIpAddress?.ToString(),
                    DeviceId = context.Request.Headers[HeaderNames.DeviceId].ToString()
                },
                context.RequestAborted);
        }
        catch (Exception loggingException)
        {
            logger.LogError(
                loggingException,
                "Failed to persist api exception log for {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
        }
    }
}
