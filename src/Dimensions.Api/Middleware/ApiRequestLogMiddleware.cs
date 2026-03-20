using Dimensions.Api.Responses;
using Dimensions.Application.Interfaces;

namespace Dimensions.Api.Middleware;

public sealed class ApiRequestLogMiddleware(RequestDelegate next, ILogger<ApiRequestLogMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IApiLogService apiLogService)
    {
        var caseId = ApiResponseFactory.GetCaseId(context);
        await apiLogService.LogRequestAsync(context.Request.Path, context.Request.Method, caseId, context.RequestAborted);

        await next(context);

        logger.LogInformation(
            "Handled request {Method} {Path} with status {StatusCode} ({CaseId})",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            caseId);
    }
}
