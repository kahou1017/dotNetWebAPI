namespace Dimensions.Admin.Web.Models.System;

public sealed class ForbiddenPageModel
{
    public string Title { get; init; } = "權限不足";

    public string Message { get; init; } = "你目前沒有權限執行這個操作。";

    public string? ErrorCode { get; init; }

    public string? CaseId { get; init; }
}
