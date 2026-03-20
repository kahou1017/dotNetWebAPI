namespace Dimensions.Application.Models;

public sealed record JwtTokenRequest
{
    public string UserId { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public string Scope { get; init; } = string.Empty;

    public string TokenType { get; init; } = string.Empty;

    public string TokenId { get; init; } = string.Empty;

    public string JwtId { get; init; } = string.Empty;

    public DateTimeOffset IssuedAt { get; init; }

    public DateTimeOffset EffectiveAt { get; init; }

    public DateTimeOffset? ExpiresAt { get; init; }
}
