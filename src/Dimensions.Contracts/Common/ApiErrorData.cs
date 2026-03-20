namespace Dimensions.Contracts.Common;

public sealed record ApiErrorData(
    string ErrorCode,
    string ErrorMessage,
    IReadOnlyDictionary<string, string[]>? ValidationErrors = null);
