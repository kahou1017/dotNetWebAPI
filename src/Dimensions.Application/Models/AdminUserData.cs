namespace Dimensions.Application.Models;

public sealed record AdminUserData
{
    public string UserId { get; init; } = string.Empty;

    public string LoginAccount { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public DateTimeOffset? LastLoginAt { get; init; }
}
