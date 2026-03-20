using Dimensions.Admin.Api.Constants;
using Dimensions.Domain.Constants;

namespace Dimensions.Admin.Api.Middleware;

public sealed class CaseIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var caseId = Guid.NewGuid().ToString("D");
        context.Items[ApiContextItemKeys.CaseId] = caseId;
        context.Response.Headers[HeaderNames.CaseId] = caseId;

        await next(context);
    }
}

