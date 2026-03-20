using Dimensions.Admin.Api.Responses;
using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Domain.Constants;

namespace Dimensions.Admin.Api.Middleware;

public sealed class ApiRequestLogMiddleware(RequestDelegate next, ILogger<ApiRequestLogMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IApiLogService apiLogService)
    {
        var caseId = ApiResponseFactory.GetCaseId(context);
        await next(context);
        await apiLogService.LogRequestAsync(
            new ApiRequestLogEntry
            {
                CaseId = caseId,
                Path = context.Request.Path,
                Method = context.Request.Method,
                StatusCode = context.Response.StatusCode,
                ClientIp = context.Connection.RemoteIpAddress?.ToString(),
                DeviceId = context.Request.Headers[HeaderNames.DeviceId].ToString()
            },
            context.RequestAborted);

        logger.LogInformation(
            "Handled request {Method} {Path} with status {StatusCode} ({CaseId})",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            caseId);
    }
}

