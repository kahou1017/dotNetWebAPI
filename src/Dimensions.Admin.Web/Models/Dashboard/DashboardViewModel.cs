namespace Dimensions.Admin.Web.Models.Dashboard;

public sealed class DashboardViewModel
{
    public string DisplayName { get; init; } = string.Empty;

    public string UserId { get; init; } = string.Empty;

    public string? LoginAccount { get; init; }
}
