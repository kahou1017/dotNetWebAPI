namespace Dimensions.Contracts.Auth;

public sealed record CurrentAdminResponse
{
    public string UserId { get; init; } = string.Empty;

    public string LoginAccount { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public DateTimeOffset LastLoginAt { get; init; }
}
