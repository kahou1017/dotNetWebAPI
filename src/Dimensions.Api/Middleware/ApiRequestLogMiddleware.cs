using Dimensions.Api.Responses;
using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Domain.Constants;

namespace Dimensions.Api.Middleware;

public sealed class ApiRequestLogMiddleware(RequestDelegate next, ILogger<ApiRequestLogMiddleware> logger)
{
    public async Task InvokeAsync(
        HttpContext context,
        IApiRequestLogService apiRequestLogService,
        ITokenUsageLogService tokenUsageLogService)
    {
        var caseId = ApiResponseFactory.GetCaseId(context);
        var requestTime = DateTimeOffset.UtcNow;
        await next(context);
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

        await apiRequestLogService.LogAsync(entry, context.RequestAborted);
        await tokenUsageLogService.LogRequestAsync(entry, context.RequestAborted);

        logger.LogInformation(
            "Handled request {Method} {Path} with status {StatusCode} ({CaseId})",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            caseId);
    }
}
