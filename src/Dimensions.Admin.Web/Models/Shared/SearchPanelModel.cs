namespace Dimensions.Admin.Web.Models.Shared;

public sealed class SearchPanelModel
{
    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string Method { get; init; } = "get";

    public string? ActionUrl { get; init; }

    public string SubmitText { get; init; } = "查詢";

    public string? ResetUrl { get; init; }

    public string FieldsPartialName { get; init; } = string.Empty;

    public object? FieldsModel { get; init; }
}
