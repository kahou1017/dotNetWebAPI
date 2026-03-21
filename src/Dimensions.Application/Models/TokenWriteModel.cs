namespace Dimensions.Application.Models;

public sealed record TokenWriteModel
{
    public string TokenId { get; init; } = string.Empty;

    public string JwtId { get; init; } = string.Empty;

    public string TokenType { get; init; } = string.Empty;

    public string UserId { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;

    public string TokenName { get; init; } = string.Empty;

    public string Scope { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public bool IsRevoked { get; init; }

    public bool IsSingleDevice { get; init; }

    public bool IsEnabled { get; init; }

    public bool CanReissue { get; init; }

    public bool CanRenew { get; init; }

    public string? DeviceId { get; init; }

    public string? DeviceName { get; init; }

    public string AccessToken { get; init; } = string.Empty;

    public DateTimeOffset IssuedAt { get; init; }

    public DateTimeOffset EffectiveAt { get; init; }

    public DateTimeOffset? ExpireAt { get; init; }

    public DateTimeOffset? LastUsedAt { get; init; }

    public string? Purpose { get; init; }

    public string? Remark { get; init; }

    public string CreatedBy { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? RevokedAt { get; init; }
}
