using Dimensions.Admin.Api.Constants;
using Dimensions.Application.Interfaces;

namespace Dimensions.Admin.Api.Services;

internal sealed class HttpContextCaseIdAccessor(IHttpContextAccessor httpContextAccessor) : ICaseIdAccessor
{
    public string GetCaseId()
    {
        var context = httpContextAccessor.HttpContext;
        if (context?.Items.TryGetValue(ApiContextItemKeys.CaseId, out var value) == true && value is string caseId)
        {
            return caseId;
        }

        return context?.TraceIdentifier ?? Guid.NewGuid().ToString("D");
    }
}

