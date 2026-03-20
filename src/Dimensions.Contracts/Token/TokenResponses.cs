using Dimensions.Contracts.Common;

namespace Dimensions.Contracts.Token;

public sealed record TokenListItemResponse
{
    public string TokenId { get; init; } = string.Empty;
    public string TokenType { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string TokenName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsSingleDevice { get; init; }
    public string? DeviceId { get; init; }
    public DateTimeOffset IssuedAt { get; init; }
    public DateTimeOffset EffectiveAt { get; init; }
    public DateTimeOffset? ExpireAt { get; init; }
    public DateTimeOffset? LastUsedAt { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
}

public sealed record TokenDetailResponse
{
    public string TokenId { get; init; } = string.Empty;
    public string JwtId { get; init; } = string.Empty;
    public string TokenType { get; init; } = string.Empty;
    public string TokenName { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsRevoked { get; init; }
    public bool IsSingleDevice { get; init; }
    public bool IsEnabled { get; init; }
    public bool CanReissue { get; init; }
    public bool CanRenew { get; init; }
    public string? DeviceId { get; init; }
    public string? DeviceName { get; init; }
    public DateTimeOffset IssuedAt { get; init; }
    public DateTimeOffset EffectiveAt { get; init; }
    public DateTimeOffset? ExpireAt { get; init; }
    public DateTimeOffset? LastUsedAt { get; init; }
    public string? Purpose { get; init; }
    public string? Remark { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed record CreateTokenResponse
{
    public string TokenId { get; init; } = string.Empty;
    public string JwtId { get; init; } = string.Empty;
    public string TokenType { get; init; } = string.Empty;
    public string AccessToken { get; init; } = string.Empty;
    public DateTimeOffset IssuedAt { get; init; }
    public DateTimeOffset EffectiveAt { get; init; }
    public DateTimeOffset? ExpireAt { get; init; }
    public string Status { get; init; } = string.Empty;
}

public sealed record RevokeTokenResponse
{
    public string TokenId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset RevokedAt { get; init; }
}

public sealed record ReissueTokenResponse
{
    public string OldTokenId { get; init; } = string.Empty;
    public string NewTokenId { get; init; } = string.Empty;
    public string NewJwtId { get; init; } = string.Empty;
    public string AccessToken { get; init; } = string.Empty;
    public string OldStatus { get; init; } = string.Empty;
    public string NewStatus { get; init; } = string.Empty;
}

public sealed record RenewTokenResponse
{
    public string TokenId { get; init; } = string.Empty;
    public string JwtId { get; init; } = string.Empty;
    public string AccessToken { get; init; } = string.Empty;
    public DateTimeOffset? OldExpireAt { get; init; }
    public DateTimeOffset? NewExpireAt { get; init; }
    public string Status { get; init; } = string.Empty;
}

public sealed record TokenUsageItemResponse
{
    public string CaseId { get; init; } = string.Empty;
    public string TokenId { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public DateTimeOffset RequestTime { get; init; }
    public string HttpMethod { get; init; } = string.Empty;
    public string RequestPath { get; init; } = string.Empty;
    public string? ClientIp { get; init; }
    public string? DeviceId { get; init; }
    public bool IsSuccess { get; init; }
    public string? FailureReason { get; init; }
}

public sealed record TokenActionLogItemResponse
{
    public string CaseId { get; init; } = string.Empty;
    public string TokenId { get; init; } = string.Empty;
    public string ActionType { get; init; } = string.Empty;
    public string? ActionReason { get; init; }
    public string ActionResult { get; init; } = string.Empty;
    public string OperatorUserId { get; init; } = string.Empty;
    public string OperatorUserName { get; init; } = string.Empty;
    public string? BeforeStatus { get; init; }
    public string? AfterStatus { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
