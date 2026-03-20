using Dimensions.Api.Constants;
using Dimensions.Contracts.Common;
using Dimensions.Domain.Constants;

namespace Dimensions.Api.Responses;

internal static class ApiResponseFactory
{
    public static ApiResponse<T> Success<T>(HttpContext context, T data, string systemCode)
    {
        return new ApiResponse<T>
        {
            Timestamp = DateTimeOffset.UtcNow,
            Success = true,
            Method = context.Request.Method,
            Path = context.Request.Path.Value ?? string.Empty,
            CaseId = GetCaseId(context),
            SystemCode = systemCode,
            Data = data
        };
    }

    public static ApiResponse<object?> Error(HttpContext context, string systemCode, string errorCode, string errorMessage, IReadOnlyDictionary<string, string[]>? validationErrors = null)
    {
        return new ApiResponse<object?>
        {
            Timestamp = DateTimeOffset.UtcNow,
            Success = false,
            Method = context.Request.Method,
            Path = context.Request.Path.Value ?? string.Empty,
            CaseId = GetCaseId(context),
            SystemCode = systemCode,
            Error = new ApiErrorData(errorCode, errorMessage, validationErrors)
        };
    }

    public static string GetCaseId(HttpContext context)
    {
        if (context.Items.TryGetValue(ApiContextItemKeys.CaseId, out var value) && value is string caseId)
        {
            return caseId;
        }

        return context.TraceIdentifier;
    }
}
