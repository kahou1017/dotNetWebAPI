namespace Dimensions.Application.Models;

public sealed record JwtTokenResult
{
    public string AccessToken { get; init; } = string.Empty;

    public DateTimeOffset IssuedAt { get; init; }

    public DateTimeOffset EffectiveAt { get; init; }

    public DateTimeOffset? ExpiresAt { get; init; }
}
