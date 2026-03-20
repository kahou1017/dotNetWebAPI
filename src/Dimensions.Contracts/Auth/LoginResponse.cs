namespace Dimensions.Contracts.Auth;

public sealed record LoginResponse
{
    public string TokenType { get; init; } = string.Empty;

    public string TokenId { get; init; } = string.Empty;

    public string JwtId { get; init; } = string.Empty;

    public string AccessToken { get; init; } = string.Empty;

    public DateTimeOffset IssuedAt { get; init; }

    public DateTimeOffset EffectiveAt { get; init; }

    public DateTimeOffset? ExpireAt { get; init; }

    public string UserId { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;
}
