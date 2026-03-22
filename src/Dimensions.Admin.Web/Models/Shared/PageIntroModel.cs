namespace Dimensions.Admin.Web.Models.Shared;

public sealed class PageIntroModel
{
    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? ActionText { get; init; }

    public string? ActionUrl { get; init; }
}
