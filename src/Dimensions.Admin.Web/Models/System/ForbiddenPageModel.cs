namespace Dimensions.Admin.Web.Models.System;

public sealed class ForbiddenPageModel
{
    public string Title { get; init; } = "沒有操作權限";

    public string Message { get; init; } = "你目前沒有權限執行這個操作，請確認帳號權限或重新登入後再試。";

    public string? ErrorCode { get; init; }

    public string? CaseId { get; init; }
}
