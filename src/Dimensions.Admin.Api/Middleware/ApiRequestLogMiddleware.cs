using System.Text;
using Dimensions.Admin.Api.Responses;
using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Domain.Constants;

namespace Dimensions.Admin.Api.Middleware;

public sealed class ApiRequestLogMiddleware(RequestDelegate next, ILogger<ApiRequestLogMiddleware> logger)
{
    public async Task InvokeAsync(
        HttpContext context,
        IApiRequestLogService apiRequestLogService,
        ITokenUsageLogService tokenUsageLogService,
        IApiPayloadLogService apiPayloadLogService)
    {
        var caseId = ApiResponseFactory.GetCaseId(context);
        var requestTime = DateTimeOffset.UtcNow;
        var originalResponseBody = context.Response.Body;
        await using var responseBuffer = new MemoryStream();
        context.Response.Body = responseBuffer;

        var requestPayload = await TryReadRequestPayloadAsync(context);

        try
        {
            await next(context);
        }
        finally
        {
            var responsePayload = await TryReadResponsePayloadAsync(context, responseBuffer);
            responseBuffer.Position = 0;
            await responseBuffer.CopyToAsync(originalResponseBody, context.RequestAborted);
            context.Response.Body = originalResponseBody;

            var entry = new ApiRequestLogEntry
            {
                CaseId = caseId,
                RequestTime = requestTime,
                Path = context.Request.Path.Value ?? string.Empty,
                Method = context.Request.Method,
                StatusCode = context.Response.StatusCode,
                ClientIp = context.Connection.RemoteIpAddress?.ToString(),
                DeviceId = context.Request.Headers[HeaderNames.DeviceId].ToString()
            };

            await TryLogAsync(
                () => apiRequestLogService.LogAsync(entry, context.RequestAborted),
                "api request log",
                context);
            await TryLogAsync(
                () => tokenUsageLogService.LogRequestAsync(entry, context.RequestAborted),
                "token usage log",
                context);

            if (!string.IsNullOrWhiteSpace(requestPayload))
            {
                await TryLogAsync(
                    () => apiPayloadLogService.LogAsync(
                        new ApiPayloadLogEntry
                        {
                            CaseId = caseId,
                            CreatedAt = requestTime,
                            Direction = "Request",
                            Path = entry.Path,
                            Method = entry.Method,
                            ContentType = context.Request.ContentType,
                            PayloadText = requestPayload,
                            PayloadLength = requestPayload.Length
                        },
                        context.RequestAborted),
                    "request payload log",
                    context);
            }

            if (!string.IsNullOrWhiteSpace(responsePayload))
            {
                await TryLogAsync(
                    () => apiPayloadLogService.LogAsync(
                        new ApiPayloadLogEntry
                        {
                            CaseId = caseId,
                            CreatedAt = DateTimeOffset.UtcNow,
                            Direction = "Response",
                            Path = entry.Path,
                            Method = entry.Method,
                            ContentType = context.Response.ContentType,
                            PayloadText = responsePayload,
                            PayloadLength = responsePayload.Length
                        },
                        context.RequestAborted),
                    "response payload log",
                    context);
            }

            logger.LogInformation(
                "Handled request {Method} {Path} with status {StatusCode} ({CaseId})",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                caseId);
        }
    }

    private async Task<string?> TryReadRequestPayloadAsync(HttpContext context)
    {
        if (!ShouldCapturePayload(context.Request.ContentType))
        {
            return null;
        }

        context.Request.EnableBuffering();
        context.Request.Body.Position = 0;
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var payload = await reader.ReadToEndAsync(context.RequestAborted);
        context.Request.Body.Position = 0;
        return string.IsNullOrWhiteSpace(payload) ? null : payload;
    }

    private async Task<string?> TryReadResponsePayloadAsync(HttpContext context, MemoryStream responseBuffer)
    {
        if (!ShouldCapturePayload(context.Response.ContentType) || responseBuffer.Length == 0)
        {
            return null;
        }

        responseBuffer.Position = 0;
        using var reader = new StreamReader(responseBuffer, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var payload = await reader.ReadToEndAsync(context.RequestAborted);
        responseBuffer.Position = 0;
        return string.IsNullOrWhiteSpace(payload) ? null : payload;
    }

    private static bool ShouldCapturePayload(string? contentType)
        => !string.IsNullOrWhiteSpace(contentType)
           && (contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase)
               || contentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase));

    private async Task TryLogAsync(Func<Task> callback, string logType, HttpContext context)
    {
        try
        {
            await callback();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to persist {LogType} for {Method} {Path}", logType, context.Request.Method, context.Request.Path);
        }
    }
}
