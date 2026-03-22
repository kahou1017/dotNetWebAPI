namespace Dimensions.Admin.Web.Models.Shared;

public sealed class ErrorAlertModel
{
    public string Title { get; init; } = "處理失敗";

    public string? ErrorMessage { get; init; }

    public string? ErrorCode { get; init; }

    public string? CaseId { get; init; }
}
