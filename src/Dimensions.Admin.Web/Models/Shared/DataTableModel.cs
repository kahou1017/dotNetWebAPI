namespace Dimensions.Admin.Web.Models.Shared;

public sealed class DataTableModel
{
    public bool HasItems { get; init; }

    public string TablePartialName { get; init; } = string.Empty;

    public object? TableModel { get; init; }

    public string EmptyMessage { get; init; } = "目前沒有資料。";
}
